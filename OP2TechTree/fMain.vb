Imports System.IO
Imports System.Media
Imports System.Text

' OP2TechTree
' https://github.com/leviathan400/OP2TechTree
'
' Create and modify Outpost 2 tech trees
' Default tech trees: edentek.txt, ply_tek.txt, tutortek.txt, multitek.txt
'
'
' Outpost 2: Divided Destiny is a real-time strategy video game released in 1997.

''' <summary>Main editor window: tech list on the left, tabs (General, Unit Properties,
''' Dependencies) on the right. Holds the loaded tech tree in memory and reads/writes tech files.</summary>
Public Class fMain

    ' Application info
    Public Const ApplicationBuild As String = "0045"
    Public Const ApplicationVersion As String = "0.8.0"
    Public Const ApplicationName As String = "OP2TechTree"
    Public Const ApplicationDescription As String = "Create and modify technology trees for Outpost 2: Divided Destiny."
    Public Const ApplicationWebsite As String = "https://github.com/leviathan400/OP2TechTree"

    ' Data
    Private TechList As New Dictionary(Of String, Technology)
    Private TechOrder As New List(Of String)   ' preserves original file order

    Private HeaderLines As New List(Of String)
    Private FooterLines As New List(Of String)
    Private TrailerPrefix As String = Nothing
    Private TrailerBytes As Byte() = Nothing

    Private ParseWarnings As New List(Of String)   ' problems found by the last ParseLines, shown after loading
    Private CurrentTech As Technology
    Private FilePath As String = ""
    Private IsLoading As Boolean = False
    Private _isDirty As Boolean = False
    Private _formSnapshot As String = Nothing    ' General-tab fields as last loaded or applied; see HasUnappliedEdits

    ' Marker used to locate the binary checksum tail in the stock OPU files.
    Private Const TrailerMarker As String = "; Garbage so checksum matches original:"

    ' --- Nested types (TechTree.vb refers to these as fMain.Technology / fMain.UnitProperty) ---

    ''' <summary>One tech: a BEGIN_TECH ... END_TECH block. Costs: -1 = disabled for that faction,
    ''' 0 = free / pre-researched, more = research time.</summary>
    Public Class Technology
        Public ID As String = ""
        Public Name As String = ""
        Public Category As Integer = 0
        Public EdenCost As Integer = 0
        Public PlymouthCost As Integer = 0
        Public MaxScientists As Integer = 0
        Public Lab As Integer = 0
        Public Description As String = ""
        Public Teaser As String = ""
        Public ImproveDesc As String = ""
        Public Prerequisites As New List(Of String)
        Public UnitProperties As New List(Of UnitProperty)

        ' Round-trip preservation
        Public LeadingComments As New List(Of String)
        Public ExtraLines As New List(Of String)   ' raw in-body comments and unrecognised tags (PLAYER_PROP, FUNCTION_RESULT, ...)
        Public UsesUnifiedCost As Boolean = True   ' true → emit COST, false → emit EDEN_COST/PLYMOUTH_COST

        ' Availability per faction: a cost of -1 means the faction can't research the tech.
        Public ReadOnly Property IsEdenAvailable As Boolean
            Get
                Return EdenCost >= 0
            End Get
        End Property

        Public ReadOnly Property IsPlymouthAvailable As Boolean
            Get
                Return PlymouthCost >= 0
            End Get
        End Property

        Public ReadOnly Property IsBothFactions As Boolean
            Get
                Return IsEdenAvailable AndAlso IsPlymouthAvailable
            End Get
        End Property

        ''' <summary>List text, "ID - Name (Both|Eden|Plymouth)". Several handlers read the ID back
        ''' as the text before the first space, so keep the ID first.</summary>
        Public Function GetDisplayText() As String
            Dim availability As String = ""
            If IsBothFactions Then
                availability = " (Both)"
            ElseIf IsEdenAvailable Then
                availability = " (Eden)"
            ElseIf IsPlymouthAvailable Then
                availability = " (Plymouth)"
            End If
            Return $"{ID} - {Name}{availability}"
        End Function
    End Class

    ''' <summary>One UNIT_PROP line: unit type, property name and value (kept as text).</summary>
    Public Class UnitProperty
        Public UnitType As String = ""
        Public PropertyName As String = ""
        Public Value As String = ""
    End Class

    ' ----- Lifecycle -----

    ''' <summary>Startup: fill the combo boxes, tooltips and context menu, start with an empty tree, play the lab sound.</summary>
    Private Sub fMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.WriteLine("--- " & ApplicationName & " Started ---")
        Try
            Me.Icon = My.Resources.Tech_Science
        Catch
            ' Resource missing; non-fatal.
        End Try

        InitializeComboBoxes()
        InitializeAdvancedFeatures()
        InitializeContextMenus()
        ClearForm()
        ClearDirty()
        UpdateStatusBar()
        UpdateTitle()

        Try
            Dim player As New SoundPlayer(My.Resources.lab_3)
            player.Play()
        Catch
            ' Sound resource missing or audio unavailable; skip silently.
        End Try
    End Sub

    ''' <summary>Fill the category, lab, unit type and unit property drop-downs.</summary>
    Private Sub InitializeComboBoxes()
        Dim categories() As String = {
            "0 - No Icon / Blank / Free",
            "1 - Lab Icon / Database Recovery",
            "2 - Factory Icon / Infrastructure",
            "3 - Power Icon / Power Generation",
            "4 - Vehicle Icon / Vehicles & Robotics",
            "5 - Food Icon / Food Production",
            "6 - Mining Icon / Mining & Metals",
            "7 - Weapons Icon / Weapons",
            "8 - Spaceport Icon / Space Program",
            "9 - Morale Icon / Population",
            "10 - Geological Icon / Disasters",
            "11 - Emergency Icon / Medical & Emergency",
            "12 - Starship Icon / Starship Components"
        }

        cmbCategory.Items.Clear()
        cmbCategory.Items.AddRange(categories)

        cmbCategoryFilter.Items.Clear()
        cmbCategoryFilter.Items.Add("All Categories")
        cmbCategoryFilter.Items.AddRange(categories)
        cmbCategoryFilter.SelectedIndex = 0

        cmbLab.Items.Clear()
        cmbLab.Items.AddRange({"None", "1 - Basic Lab", "2 - Standard Lab", "3 - Advanced Lab"})

        cmbUnitType.Items.Clear()
        cmbUnitType.Items.AddRange(TechConstants.UnitTypes)

        cmbUnitProperty.Items.Clear()
        cmbUnitProperty.Items.AddRange(TechConstants.UnitProperties)
    End Sub

    ''' <summary>Tooltips explaining the value rules of the main fields.</summary>
    Private Sub InitializeAdvancedFeatures()
        ToolTip1.SetToolTip(txtTechID, "5-digit Tech ID. Format: XXYYZ where XX*1000 = tech level. Changing this re-keys the tech and updates all REQUIRES references.")
        ToolTip1.SetToolTip(cmbCategory, "Technology category icon. 0 = no icon (used for free/disabled techs and tech-level markers).")
        ToolTip1.SetToolTip(txtMaxScientists, "Maximum scientists per topic. Valid: 1 (special non-lab tech) or 4-18 inclusive.")
        ToolTip1.SetToolTip(cmbLab, "Lab required. 0 = none (free / disabled techs), 1 = Basic, 2 = Standard, 3 = Advanced.")
        ToolTip1.SetToolTip(txtEdenCost, "Research time for Eden. -1 = disabled, 0 = pre-researched/free, >0 = research time.")
        ToolTip1.SetToolTip(txtPlymouthCost, "Research time for Plymouth. -1 = disabled, 0 = pre-researched/free, >0 = research time.")
        ToolTip1.SetToolTip(chkEden, "Available to Eden (cost >= 0).")
        ToolTip1.SetToolTip(chkPlymouth, "Available to Plymouth (cost >= 0).")
        ToolTip1.SetToolTip(chkBoth, "Available to both factions.")
        ToolTip1.SetToolTip(btnSaveTech, "Apply current edits to the in-memory tech tree. Use File > Save to persist to disk.")
    End Sub

    Private ctxTechList As ContextMenuStrip
    Private ctxPrereqList As ContextMenuStrip

    ''' <summary>Right-click menu on the tech list: Find References and View Raw File.</summary>
    Private Sub InitializeContextMenus()
        ctxTechList = New ContextMenuStrip()
        Dim findRefs As New ToolStripMenuItem("Find References (techs that require this)")
        AddHandler findRefs.Click, AddressOf TechListFindReferences_Click
        ctxTechList.Items.Add(findRefs)

        Dim viewRaw As New ToolStripMenuItem("View Raw File (what would be written)")
        AddHandler viewRaw.Click, AddressOf ViewRawFile_Click
        ctxTechList.Items.Add(viewRaw)

        lstTechnologies.ContextMenuStrip = ctxTechList
    End Sub

    ''' <summary>Empty every field on the form and drop the current tech (nothing is selected afterwards).</summary>
    Private Sub ClearForm()
        CurrentTech = Nothing
        IsLoading = True

        txtSearch.Text = ""
        cmbCategoryFilter.SelectedIndex = 0
        txtTechID.Text = ""
        txtTechName.Text = ""
        cmbCategory.SelectedIndex = 0
        txtMaxScientists.Text = ""
        cmbLab.SelectedIndex = 0
        txtEdenCost.Text = ""
        txtPlymouthCost.Text = ""
        chkEden.Checked = False
        chkPlymouth.Checked = False
        chkBoth.Checked = False
        txtTeaser.Text = ""
        txtDescription.Text = ""
        txtImproveDesc.Text = ""
        lstPrerequisites.Items.Clear()
        lstUnitProperties.Items.Clear()
        cmbUnitType.SelectedIndex = -1
        cmbUnitProperty.SelectedIndex = -1
        txtUnitValue.Text = ""
        UpdateDependenciesTab()

        IsLoading = False
    End Sub

    ' ----- Dirty tracking -----

    ''' <summary>Flag unsaved changes; the title bar shows a *.</summary>
    Private Sub MarkDirty()
        If Not _isDirty Then
            _isDirty = True
            UpdateTitle()
        End If
    End Sub

    ''' <summary>Clear the unsaved-changes flag (after load, save or close).</summary>
    Private Sub ClearDirty()
        If _isDirty Then
            _isDirty = False
            UpdateTitle()
        Else
            UpdateTitle()
        End If
    End Sub

    ''' <summary>Title bar: file name (or "New File") plus * when there are unsaved changes.</summary>
    Private Sub UpdateTitle()
        Dim baseName As String = If(String.IsNullOrEmpty(FilePath), "New File", Path.GetFileName(FilePath))
        Dim dirtyMark As String = If(_isDirty, " *", "")
        Me.Text = $"Outpost 2 Tech Tree Editor - {baseName}{dirtyMark}"
        ' UpdateTitle runs on every load/new/edit, so this keeps Close (and its Ctrl+W shortcut) current.
        CloseToolStripMenuItem.Enabled = IsTechTreeOpen()
    End Sub

    ''' <summary>True when a file is loaded or the tree has content (e.g. a new, unsaved tree).</summary>
    Private Function IsTechTreeOpen() As Boolean
        Return Not String.IsNullOrEmpty(FilePath) OrElse TechList.Count > 0
    End Function

    ''' <summary>Prompt user to save if dirty or the form has edits not yet applied. Returns False if
    ''' user cancels.</summary>
    Private Function PromptSaveChanges() As Boolean
        If Not _isDirty AndAlso Not HasUnappliedEdits() Then Return True

        Dim result = MessageBox.Show(
            "You have unsaved changes. Save before continuing?",
            "Unsaved Changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning)

        Select Case result
            Case DialogResult.Yes
                If Not SaveCurrentTech() Then Return False
                If String.IsNullOrEmpty(FilePath) Then
                    ' Need a target file
                    If SaveFileDialog1.ShowDialog() <> DialogResult.OK Then Return False
                    FilePath = SaveFileDialog1.FileName
                End If
                Return SaveTechFile(FilePath)
            Case DialogResult.No
                Return True
            Case Else
                Return False
        End Select
    End Function

    ' ----- File menu -----

    ''' <summary>File > New: same as closing the tech tree.</summary>
    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        CloseTechTree()
    End Sub

    ''' <summary>File > Close Tech Tree (Ctrl+W). Does nothing when no tech tree is open.</summary>
    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        If IsTechTreeOpen() Then CloseTechTree()
    End Sub

    ''' <summary>Offer to save unsaved changes, then clear back to an empty, untitled tech tree.
    ''' Returns False if the user cancelled.</summary>
    Private Function CloseTechTree() As Boolean
        If Not PromptSaveChanges() Then Return False
        TechList.Clear()
        TechOrder.Clear()
        HeaderLines.Clear()
        FooterLines.Clear()
        TrailerPrefix = Nothing
        TrailerBytes = Nothing
        FilePath = ""
        ClearForm()
        PopulateTechList()
        ClearDirty()
        UpdateTitle()
        UpdateStatusBar()
        Return True
    End Function

    ''' <summary>File > Open: offer to save changes, then load the chosen file.</summary>
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If Not PromptSaveChanges() Then Return
        If OpenFileDialog1.ShowDialog() <> DialogResult.OK Then Return
        LoadTechFile(OpenFileDialog1.FileName)
    End Sub

    ''' <summary>File > Save: apply the form, then write the file (asks for a name if it has none).</summary>
    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If Not SaveCurrentTech() Then Return
        If String.IsNullOrEmpty(FilePath) Then
            SaveAsToolStripMenuItem_Click(sender, e)
        Else
            SaveTechFile(FilePath)
        End If
    End Sub

    ''' <summary>File > Save As: apply the form, then write to a new file name.</summary>
    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        If Not SaveCurrentTech() Then Return
        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            If SaveTechFile(SaveFileDialog1.FileName) Then
                FilePath = SaveFileDialog1.FileName
                UpdateTitle()
            End If
        End If
    End Sub

    ' ----- File I/O -----

    ''' <summary>Read a tech file. The checksum trailer of the stock OPU files is kept as raw bytes;
    ''' the text is decoded as Latin-1 and parsed. Problems found while parsing are listed afterwards.</summary>
    Private Sub LoadTechFile(fileName As String)
        Try
            ' Parse into locals first; current state is only replaced once the whole file has parsed,
            ' so a failed load can't leave a half-empty tree pointing at the old FilePath.
            Dim newTrailerPrefix As String = Nothing
            Dim newTrailerBytes As Byte() = Nothing

            ' Read raw bytes so we can preserve the OPU checksum trailer verbatim.
            Dim raw As Byte() = File.ReadAllBytes(fileName)
            Dim markerBytes As Byte() = Encoding.ASCII.GetBytes(TrailerMarker)
            Dim markerIndex As Integer = IndexOfBytes(raw, markerBytes)

            Dim textBytes As Byte()
            If markerIndex >= 0 Then
                Dim trailerStart As Integer = markerIndex + markerBytes.Length
                textBytes = New Byte(markerIndex - 1) {}
                Array.Copy(raw, 0, textBytes, 0, markerIndex)
                newTrailerPrefix = TrailerMarker
                newTrailerBytes = New Byte(raw.Length - trailerStart - 1) {}
                Array.Copy(raw, trailerStart, newTrailerBytes, 0, newTrailerBytes.Length)
            Else
                textBytes = raw
            End If

            ' Latin-1 decode never fails; preserves bytes 0-255 as chars 0-255.
            Dim text As String = Encoding.GetEncoding("ISO-8859-1").GetString(textBytes)
            Dim lines As List(Of String) = text.Split({vbCrLf, vbLf}, StringSplitOptions.None).ToList()
            ' A trailing newline yields one empty element that isn't a real line; AppendLine re-adds it on save.
            If lines.Count > 0 AndAlso lines(lines.Count - 1).Length = 0 Then lines.RemoveAt(lines.Count - 1)

            Dim newTechs As New Dictionary(Of String, Technology)
            Dim newOrder As New List(Of String)
            Dim newHeader As New List(Of String)
            Dim newFooter As New List(Of String)
            ParseLines(lines, newTechs, newOrder, newHeader, newFooter)

            TechList = newTechs
            TechOrder = newOrder
            HeaderLines = newHeader
            FooterLines = newFooter
            TrailerPrefix = newTrailerPrefix
            TrailerBytes = newTrailerBytes

            FilePath = fileName
            ClearForm()   ' drop CurrentTech so the previous file's tech can't be re-applied into this one
            PopulateTechList()
            ClearDirty()
            UpdateTitle()
            UpdateStatusBar()

            Debug.WriteLine("File Loaded: " & Path.GetFileName(fileName))
            If ParseWarnings.Count = 0 Then
                MessageBox.Show($"Loaded {TechList.Count} technologies from {Path.GetFileName(fileName)}.",
                    "File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Const MaxShown As Integer = 10
                Dim msg As New StringBuilder($"Loaded {TechList.Count} technologies from {Path.GetFileName(fileName)}, with problems:{vbCrLf}{vbCrLf}")
                For Each w In ParseWarnings.Take(MaxShown)
                    msg.AppendLine(" • " & w)
                Next
                If ParseWarnings.Count > MaxShown Then msg.AppendLine($" • ...and {ParseWarnings.Count - MaxShown} more.")
                MessageBox.Show(msg.ToString(), "File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading file: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Position of `needle` in `haystack`, or -1.</summary>
    Private Shared Function IndexOfBytes(haystack As Byte(), needle As Byte()) As Integer
        If needle Is Nothing OrElse needle.Length = 0 Then Return -1
        For i As Integer = 0 To haystack.Length - needle.Length
            Dim match As Boolean = True
            For j As Integer = 0 To needle.Length - 1
                If haystack(i + j) <> needle(j) Then
                    match = False
                    Exit For
                End If
            Next
            If match Then Return i
        Next
        Return -1
    End Function

    ''' <summary>Tokenize a line on any run of whitespace (space/tab) into a tag and the remainder.</summary>
    Private Shared Function SplitFirstToken(line As String) As Tuple(Of String, String)
        Dim i As Integer = 0
        While i < line.Length AndAlso Char.IsWhiteSpace(line(i))
            i += 1
        End While
        Dim tokenStart As Integer = i
        While i < line.Length AndAlso Not Char.IsWhiteSpace(line(i))
            i += 1
        End While
        Dim token As String = line.Substring(tokenStart, i - tokenStart)
        Dim rest As String = If(i < line.Length, line.Substring(i).Trim(), "")
        Return Tuple.Create(token, rest)
    End Function

    ' Tags whose values this editor reads. Their inline comments are split off before parsing;
    ' unknown tags are kept as raw lines (comment included), so they're left alone.
    Private Shared ReadOnly KnownTags As New HashSet(Of String) From {
        "BEGIN_TECH", "END_TECH", "CATEGORY", "COST", "EDEN_COST", "PLYMOUTH_COST", "MAX_SCIENTISTS",
        "LAB", "DESCRIPTION", "TEASER", "IMPROVE_DESC", "REQUIRES", "UNIT_PROP"}

    ''' <summary>Split an inline "; comment" off a tag's value. A ';' inside a quoted string is text,
    ''' not a comment. Returns the value; `comment` gets the comment (from the ';') or Nothing.</summary>
    Private Shared Function StripInlineComment(rest As String, ByRef comment As String) As String
        comment = Nothing
        Dim inQuote As Boolean = False
        For i As Integer = 0 To rest.Length - 1
            If rest(i) = """"c Then
                inQuote = Not inQuote
            ElseIf rest(i) = ";"c AndAlso Not inQuote Then
                comment = rest.Substring(i).TrimEnd()
                Return rest.Substring(0, i).TrimEnd()
            End If
        Next
        Return rest
    End Function

    ''' <summary>A line starting with a tag as written in tech files (upper case: COST, END_TECH, ...).
    ''' An unclosed quoted string stops before it rather than swallowing the tag.</summary>
    Private Shared Function IsTagLine(line As String) As Boolean
        Return KnownTags.Contains(SplitFirstToken(line).Item1)
    End Function

    ''' <summary>Pull a quoted string starting from the first " in `firstLine`. If unclosed,
    ''' continue reading subsequent lines (consuming them via byRef index). Embedded newlines/tabs
    ''' are stripped to match OP2's parser semantics. A string still open at the next tag line
    ''' (or the end of the file) stops there and sets `unclosed`, rather than eating the rest of the file.</summary>
    Private Shared Function ReadQuotedString(firstLine As String, ByRef lineIdx As Integer, lines As List(Of String),
                                             ByRef unclosed As Boolean) As String
        unclosed = False
        Dim startQuote As Integer = firstLine.IndexOf(""""c)
        If startQuote < 0 Then Return firstLine.Trim()

        Dim sb As New StringBuilder()
        Dim endQuote As Integer = firstLine.IndexOf(""""c, startQuote + 1)
        If endQuote >= 0 Then
            Return firstLine.Substring(startQuote + 1, endQuote - startQuote - 1).Replace(vbTab, " ")
        End If

        ' Multi-line: append remainder of first line, then keep reading.
        sb.Append(firstLine.Substring(startQuote + 1).Replace(vbTab, " "))
        While lineIdx + 1 < lines.Count
            If IsTagLine(lines(lineIdx + 1)) Then Exit While   ' leave that line for ParseLines
            lineIdx += 1
            Dim ln As String = lines(lineIdx)
            Dim close As Integer = ln.IndexOf(""""c)
            If close >= 0 Then
                sb.Append(" ")
                sb.Append(ln.Substring(0, close).Replace(vbTab, " "))
                Return sb.ToString()
            Else
                sb.Append(" ")
                sb.Append(ln.Replace(vbTab, " "))
            End If
        End While
        unclosed = True
        Return sb.ToString().TrimEnd()
    End Function

    ''' <summary>Read a DESCRIPTION / TEASER / IMPROVE_DESC value, noting a missing closing quote.</summary>
    Private Function ReadTagText(tech As Technology, tag As String, rest As String,
                                 lines As List(Of String), ByRef lineIdx As Integer) As String
        Dim tagLine As Integer = lineIdx + 1
        Dim unclosed As Boolean
        Dim text As String = ReadQuotedString(rest, lineIdx, lines, unclosed)
        If unclosed Then
            ParseWarnings.Add($"Line {tagLine}: {tag} of tech {tech.ID} ""{tech.Name}"" has no closing quote. " &
                              "Its text was read up to the next tag; check it before saving.")
        End If
        Return text
    End Function

    ''' <summary>Drop blank lines at the start of a comment block. EmitTech already writes one blank
    ''' line after every END_TECH, so keeping these would add a line per tech on every save.</summary>
    Private Shared Function TrimLeadingBlankLines(block As List(Of String)) As List(Of String)
        Return block.SkipWhile(Function(l) l.Trim().Length = 0).ToList()
    End Function

    ''' <summary>Parse tech file lines into `techs` (by ID) and `order` (file order). Comments before
    ''' the first tech go to `header`, after the last to `footer`; comments between techs stay with
    ''' the following tech. Problems are added to ParseWarnings.</summary>
    Private Sub ParseLines(lines As List(Of String),
                           techs As Dictionary(Of String, Technology), order As List(Of String),
                           header As List(Of String), footer As List(Of String))
        ParseWarnings = New List(Of String)
        Dim pendingComments As New List(Of String)   ' comments awaiting attachment
        Dim currentTech As Technology = Nothing
        Dim sawAnyTech As Boolean = False
        Dim hasExplicitEden As Boolean = False
        Dim hasExplicitPly As Boolean = False
        Dim hasUnifiedCost As Boolean = False
        Dim beginLine As Integer = 0                 ' 1-based line of the current BEGIN_TECH, for warnings

        Dim i As Integer = 0
        While i < lines.Count
            Dim rawLine As String = lines(i)
            Dim trimmed As String = rawLine.TrimStart()

            ' Comment or blank
            If trimmed.Length = 0 OrElse trimmed.StartsWith(";") Then
                If currentTech Is Nothing Then
                    pendingComments.Add(rawLine)
                ElseIf trimmed.Length > 0 Then
                    ' Comment inside a tech body: keep it with that tech (re-emitted just before END_TECH).
                    currentTech.ExtraLines.Add(rawLine)
                End If
                i += 1
                Continue While
            End If

            Dim split = SplitFirstToken(rawLine)
            Dim tag As String = split.Item1.ToUpperInvariant()
            Dim rest As String = split.Item2

            ' "COST 1200 ; note" -> value "1200". The comment is kept as its own line in the tech.
            Dim inlineComment As String = Nothing
            If KnownTags.Contains(tag) Then rest = StripInlineComment(rest, inlineComment)

            Select Case tag
                Case "BEGIN_TECH"
                    beginLine = i + 1
                    Dim newTech As New Technology()
                    Dim nameStart As Integer = rest.IndexOf(""""c)
                    If nameStart >= 0 Then
                        Dim nameEnd As Integer = rest.IndexOf(""""c, nameStart + 1)
                        If nameEnd > nameStart Then
                            newTech.Name = rest.Substring(nameStart + 1, nameEnd - nameStart - 1)
                            Dim afterName As String = rest.Substring(nameEnd + 1).Trim()
                            newTech.ID = afterName
                        End If
                    End If
                    If String.IsNullOrEmpty(newTech.ID) Then
                        ParseWarnings.Add($"Line {i + 1}: BEGIN_TECH needs a quoted name followed by an ID. " &
                                          "This tech was skipped and will be missing if you save.")
                    End If

                    If Not sawAnyTech Then
                        header.AddRange(pendingComments)
                        sawAnyTech = True
                    Else
                        newTech.LeadingComments.AddRange(TrimLeadingBlankLines(pendingComments))
                    End If
                    pendingComments.Clear()

                    currentTech = newTech
                    hasExplicitEden = False
                    hasExplicitPly = False
                    hasUnifiedCost = False

                Case "END_TECH"
                    If currentTech IsNot Nothing Then
                        ' Determine cost style: only "unified" if the source actually used COST (and didn't also override).
                        currentTech.UsesUnifiedCost = hasUnifiedCost AndAlso Not hasExplicitEden AndAlso Not hasExplicitPly

                        If String.IsNullOrEmpty(currentTech.ID) Then
                            ' Skip malformed tech with no ID
                        ElseIf techs.ContainsKey(currentTech.ID) Then
                            ' Duplicate IDs: keep the later one (matches game behavior for single-value tags),
                            ' but say so, because the earlier copy is dropped from the file on save.
                            ParseWarnings.Add($"Line {beginLine}: tech ""{currentTech.Name}"" reuses ID {currentTech.ID}, " &
                                              $"already used by ""{techs(currentTech.ID).Name}"". Only this later one is kept; " &
                                              "the earlier one will be missing if you save.")
                            techs(currentTech.ID) = currentTech
                        Else
                            techs(currentTech.ID) = currentTech
                            order.Add(currentTech.ID)
                        End If
                        currentTech = Nothing
                    End If

                Case Else
                    If currentTech Is Nothing Then
                        ' Stray tag outside a tech block: keep it as a header/comment line
                        pendingComments.Add(rawLine)
                    Else
                        ApplyTag(currentTech, tag, rest, lines, i, hasExplicitEden, hasExplicitPly, hasUnifiedCost)
                    End If
            End Select

            If inlineComment IsNot Nothing Then
                If currentTech IsNot Nothing Then
                    currentTech.ExtraLines.Add("    " & inlineComment)
                Else
                    pendingComments.Add(inlineComment)   ' from an END_TECH line: goes before the next tech
                End If
            End If

            i += 1
        End While

        ' Anything left in pendingComments after the last END_TECH is footer
        If sawAnyTech Then
            footer.AddRange(TrimLeadingBlankLines(pendingComments))
        Else
            header.AddRange(pendingComments)
        End If
    End Sub

    ''' <summary>Apply one tag line inside a tech. The ByRef flags record which cost tags were used,
    ''' so the tech is written back with COST or with EDEN_COST / PLYMOUTH_COST as before.</summary>
    Private Sub ApplyTag(tech As Technology, tag As String, rest As String,
                         lines As List(Of String), ByRef lineIdx As Integer,
                         ByRef hasExplicitEden As Boolean, ByRef hasExplicitPly As Boolean,
                         ByRef hasUnifiedCost As Boolean)
        Select Case tag
            Case "CATEGORY"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then tech.Category = v

            Case "COST"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then
                    tech.EdenCost = v
                    tech.PlymouthCost = v
                    hasUnifiedCost = True
                End If

            Case "EDEN_COST"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then
                    tech.EdenCost = v
                    hasExplicitEden = True
                End If

            Case "PLYMOUTH_COST"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then
                    tech.PlymouthCost = v
                    hasExplicitPly = True
                End If

            Case "MAX_SCIENTISTS"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then tech.MaxScientists = v

            Case "LAB"
                Dim v As Integer
                If Integer.TryParse(rest, v) Then tech.Lab = v

            Case "DESCRIPTION"
                tech.Description = ReadTagText(tech, tag, rest, lines, lineIdx)

            Case "TEASER"
                tech.Teaser = ReadTagText(tech, tag, rest, lines, lineIdx)

            Case "IMPROVE_DESC"
                tech.ImproveDesc = ReadTagText(tech, tag, rest, lines, lineIdx)

            Case "REQUIRES"
                Dim id As String = rest.Trim()
                If id.Length > 0 Then tech.Prerequisites.Add(id)

            Case "UNIT_PROP"
                ' Three whitespace-separated tokens: UnitType Property Value
                Dim parts As String() = rest.Split(New Char() {" "c, vbTab(0)},
                                                   StringSplitOptions.RemoveEmptyEntries)
                If parts.Length >= 3 Then
                    tech.UnitProperties.Add(New UnitProperty() With {
                        .UnitType = parts(0),
                        .PropertyName = parts(1),
                        .Value = String.Join(" ", parts.Skip(2))
                    })
                End If

            Case Else
                ' Unknown tag (PLAYER_PROP, FUNCTION_RESULT, ...): not editable here, but keep the raw line
                ' so saving doesn't silently delete it.
                tech.ExtraLines.Add(lines(lineIdx))
        End Select
    End Sub

    ''' <summary>Write the tech tree to `fileName`, first copying any existing file to .bak.
    ''' Returns False if writing failed.</summary>
    Private Function SaveTechFile(fileName As String) As Boolean
        ' Backup only if file already exists.
        Try
            If File.Exists(fileName) Then
                Dim backupFilePath As String = fileName & ".bak"
                If File.Exists(backupFilePath) Then File.Delete(backupFilePath)
                File.Copy(fileName, backupFilePath)
                Debug.WriteLine("Backup created: " & backupFilePath)
            End If
        Catch ex As Exception
            ' Backup is best-effort; warn but continue with save.
            Debug.WriteLine("Backup failed: " & ex.Message)
            MessageBox.Show($"Could not create backup: {ex.Message}{vbCrLf}{vbCrLf}Proceeding with save.",
                "Backup Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

        Try
            Dim rendered As String = RenderFileText()

            ' Write the text, then the binary trailer (if any). Not atomic: if writing fails part-way,
            ' the file is incomplete and the .bak copy made above is the way back.
            Dim textBytes As Byte() = Encoding.GetEncoding("ISO-8859-1").GetBytes(rendered)
            Using fs As New FileStream(fileName, FileMode.Create, FileAccess.Write)
                fs.Write(textBytes, 0, textBytes.Length)
                If TrailerPrefix IsNot Nothing Then
                    Dim prefixBytes = Encoding.ASCII.GetBytes(TrailerPrefix)
                    fs.Write(prefixBytes, 0, prefixBytes.Length)
                    If TrailerBytes IsNot Nothing AndAlso TrailerBytes.Length > 0 Then
                        fs.Write(TrailerBytes, 0, TrailerBytes.Length)
                    End If
                End If
            End Using

            ClearDirty()
            UpdateTitle()

            Debug.WriteLine("Tech file saved: " & Path.GetFileName(fileName))
            MessageBox.Show($"Tech tree saved to {Path.GetFileName(fileName)}.", "File Saved",
                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True

        Catch ex As Exception
            MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>The file text that Save writes (without the binary trailer): header, techs in
    ''' file order then newly added techs by ID, footer.</summary>
    Private Function RenderFileText() As String
        Dim sb As New StringBuilder()

        ' Header
        If HeaderLines.Count = 0 Then
            sb.AppendLine(";   Technology tree for Outpost 2")
            sb.AppendLine(";   Edited with " & ApplicationName & " " & ApplicationVersion)
            sb.AppendLine(";")
            sb.AppendLine()
        Else
            For Each line In HeaderLines
                sb.AppendLine(line)
            Next
        End If

        ' Emit techs in preserved order, then any techs that were added after load (not yet in TechOrder)
        Dim emitted As New HashSet(Of String)

        For Each id In TechOrder
            If TechList.ContainsKey(id) Then
                EmitTech(sb, TechList(id))
                emitted.Add(id)
            End If
        Next

        ' Newly added techs not in TechOrder yet (sort by ID for stable output)
        Dim leftovers = TechList.Values.Where(Function(t) Not emitted.Contains(t.ID)).OrderBy(Function(t) t.ID).ToList()
        For Each tech In leftovers
            EmitTech(sb, tech)
        Next

        ' Footer
        For Each line In FooterLines
            sb.AppendLine(line)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>Write one tech block, tags in a fixed order, followed by a blank line.</summary>
    Private Sub EmitTech(sb As StringBuilder, tech As Technology)
        For Each c In tech.LeadingComments
            sb.AppendLine(c)
        Next

        sb.AppendLine($"BEGIN_TECH ""{tech.Name}"" {tech.ID}")
        sb.AppendLine($"    CATEGORY        {tech.Category}")

        If Not String.IsNullOrEmpty(tech.Description) Then
            sb.AppendLine($"    DESCRIPTION     ""{tech.Description}""")
        End If

        If Not String.IsNullOrEmpty(tech.Teaser) Then
            sb.AppendLine($"    TEASER          ""{tech.Teaser}""")
        End If

        For Each prereq In tech.Prerequisites
            sb.AppendLine($"    REQUIRES        {prereq}")
        Next

        If tech.UsesUnifiedCost AndAlso tech.EdenCost = tech.PlymouthCost Then
            sb.AppendLine($"    COST            {tech.EdenCost}")
        Else
            sb.AppendLine($"    EDEN_COST       {tech.EdenCost}")
            sb.AppendLine($"    PLYMOUTH_COST   {tech.PlymouthCost}")
        End If

        If tech.MaxScientists > 0 Then
            sb.AppendLine($"    MAX_SCIENTISTS  {tech.MaxScientists}")
        End If

        If tech.Lab > 0 Then
            sb.AppendLine($"    LAB             {tech.Lab}")
        End If

        If Not String.IsNullOrEmpty(tech.ImproveDesc) Then
            sb.AppendLine($"    IMPROVE_DESC    ""{tech.ImproveDesc}""")
        End If

        For Each prop In tech.UnitProperties
            sb.AppendLine($"    UNIT_PROP {prop.UnitType} {prop.PropertyName} {prop.Value}")
        Next

        For Each line In tech.ExtraLines
            sb.AppendLine(line)
        Next

        sb.AppendLine("END_TECH")
        sb.AppendLine()
    End Sub

    ' ----- UI population -----

    ''' <summary>Refill the tech list, applying the search text and category filter.</summary>
    Private Sub PopulateTechList()
        lstTechnologies.Items.Clear()
        Dim searchText As String = txtSearch.Text.ToLower()
        Dim categoryFilter As Integer = -1
        If cmbCategoryFilter.SelectedIndex > 0 Then
            categoryFilter = cmbCategoryFilter.SelectedIndex - 1
        End If

        For Each tech In TechList.Values.OrderBy(Function(x) x.ID)
            Dim include As Boolean = True

            If Not String.IsNullOrEmpty(searchText) Then
                include = tech.Name.ToLower().Contains(searchText) OrElse
                         tech.ID.ToLower().Contains(searchText)
            End If

            If include AndAlso categoryFilter >= 0 Then
                include = (tech.Category = categoryFilter)
            End If

            If include Then
                lstTechnologies.Items.Add(tech.GetDisplayText())
            End If
        Next

        PopulatePrerequisiteCombo()
        UpdateStatusBar()
    End Sub

    ''' <summary>Techs that can still be added as prerequisites of the current tech.</summary>
    Private Sub PopulatePrerequisiteCombo()
        cmbAvailablePrereqs.Items.Clear()
        For Each tech In TechList.Values.OrderBy(Function(x) x.ID)
            If CurrentTech IsNot Nothing AndAlso (tech.ID = CurrentTech.ID OrElse CurrentTech.Prerequisites.Contains(tech.ID)) Then
                Continue For
            End If
            cmbAvailablePrereqs.Items.Add(tech.GetDisplayText())
        Next
    End Sub

    ''' <summary>Status bar: number of techs and total research cost per faction.</summary>
    Private Sub UpdateStatusBar()
        lblTechCount.Text = TechList.Count.ToString()

        ' Sum positive research costs per faction (consistent across all techs).
        Dim totalEden As Integer = 0
        Dim totalPly As Integer = 0
        For Each tech In TechList.Values
            If tech.EdenCost > 0 Then totalEden += tech.EdenCost
            If tech.PlymouthCost > 0 Then totalPly += tech.PlymouthCost
        Next
        lblTotalCost.Text = $"Eden {totalEden:N0} / Plymouth {totalPly:N0}"
    End Sub

    ' ----- Selection & editing -----

    ''' <summary>Show the tech picked in the list. If the General tab has edits that were not applied,
    ''' ask first: Yes applies them, No drops them, Cancel stays on the edited tech.</summary>
    Private Sub lstTechnologies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstTechnologies.SelectedIndexChanged
        If lstTechnologies.SelectedIndex < 0 OrElse IsLoading Then Return

        Dim selectedText As String = lstTechnologies.SelectedItem.ToString()
        Dim techID As String = selectedText.Split(" "c)(0)

        If CurrentTech IsNot Nothing AndAlso techID <> CurrentTech.ID AndAlso HasUnappliedEdits() Then
            Dim answer = MessageBox.Show(
                $"Apply your changes to {CurrentTech.ID} ""{CurrentTech.Name}"" before switching?",
                "Unapplied Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)
            If answer = DialogResult.Cancel OrElse (answer = DialogResult.Yes AndAlso Not SaveCurrentTech()) Then
                ReselectCurrentTech()
                Return
            End If
            If answer = DialogResult.Yes Then
                ' Applying refilled the list and reselected the edited tech; now go where the user clicked.
                SelectTechInList(techID)
                Return
            End If
        End If

        ' Stay on the Dependencies tab while browsing (if the new tech has one); otherwise back to General.
        Dim stayOnDependencies As Boolean = TabControl1.SelectedTab Is TabPage3

        If TechList.ContainsKey(techID) Then
            LoadTechToForm(TechList(techID))
        End If

        If stayOnDependencies AndAlso TabControl1.TabPages.Contains(TabPage3) Then
            TabControl1.SelectedTab = TabPage3
        Else
            TabControl1.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>Show a tech on the form (all tabs). IsLoading stops the change handlers from reacting.</summary>
    Private Sub LoadTechToForm(tech As Technology)
        IsLoading = True
        CurrentTech = tech

        txtTechID.Text = tech.ID
        txtTechName.Text = tech.Name
        cmbCategory.SelectedIndex = Math.Min(Math.Max(tech.Category, 0), cmbCategory.Items.Count - 1)
        txtMaxScientists.Text = If(tech.MaxScientists > 0, tech.MaxScientists.ToString(), "")
        cmbLab.SelectedIndex = Math.Min(Math.Max(tech.Lab, 0), cmbLab.Items.Count - 1)
        txtEdenCost.Text = tech.EdenCost.ToString()
        txtPlymouthCost.Text = tech.PlymouthCost.ToString()

        chkEden.Checked = tech.IsEdenAvailable
        chkPlymouth.Checked = tech.IsPlymouthAvailable
        chkBoth.Checked = tech.IsBothFactions

        txtTeaser.Text = tech.Teaser
        txtDescription.Text = tech.Description
        txtImproveDesc.Text = tech.ImproveDesc

        UpdateCategoryIcon()
        RefreshTechLists()
        _formSnapshot = FormSnapshot()

        IsLoading = False
    End Sub

    ''' <summary>Refill the current tech's Prerequisites, Unit Properties and Dependencies views.
    ''' Those edits apply at once, so this leaves the General tab (and any unapplied edits) alone.</summary>
    Private Sub RefreshTechLists()
        Dim tech As Technology = CurrentTech
        If tech Is Nothing Then Return
        Dim wasLoading As Boolean = IsLoading
        IsLoading = True

        lstPrerequisites.Items.Clear()
        For Each prereq In tech.Prerequisites
            If TechList.ContainsKey(prereq) Then
                lstPrerequisites.Items.Add(TechList(prereq).GetDisplayText())
            Else
                lstPrerequisites.Items.Add(prereq & " (Missing!)")
            End If
        Next

        lstUnitProperties.Items.Clear()
        For Each prop In tech.UnitProperties
            Dim item As New ListViewItem(prop.UnitType)
            item.SubItems.Add(prop.PropertyName)
            item.SubItems.Add(prop.Value)
            lstUnitProperties.Items.Add(item)
        Next

        PopulatePrerequisiteCombo()
        UpdateDependenciesTab()

        IsLoading = wasLoading
    End Sub

    ''' <summary>The General-tab fields that Apply writes to the tech, as one string. Taken from the
    ''' controls themselves, so it compares like with like whatever the controls do to the text.</summary>
    Private Function FormSnapshot() As String
        Return String.Join(vbNullChar, txtTechID.Text, txtTechName.Text, cmbCategory.SelectedIndex.ToString(),
                           txtMaxScientists.Text, cmbLab.SelectedIndex.ToString(), txtEdenCost.Text, txtPlymouthCost.Text,
                           txtTeaser.Text, txtDescription.Text, txtImproveDesc.Text)
    End Function

    ''' <summary>True when the General tab was edited since the tech was loaded or last applied.</summary>
    Private Function HasUnappliedEdits() As Boolean
        Return CurrentTech IsNot Nothing AndAlso _formSnapshot IsNot Nothing AndAlso FormSnapshot() <> _formSnapshot
    End Function

    ''' <summary>Put the list selection back on the current tech without reloading the form, so its
    ''' unapplied edits stay. Clears the selection if a filter hides the tech.</summary>
    Private Sub ReselectCurrentTech()
        IsLoading = True
        Try
            lstTechnologies.SelectedIndex = -1
            For i As Integer = 0 To lstTechnologies.Items.Count - 1
                If lstTechnologies.Items(i).ToString().StartsWith(CurrentTech.ID & " ") Then
                    lstTechnologies.SelectedIndex = i
                    Exit For
                End If
            Next
        Finally
            IsLoading = False
        End Try
    End Sub

    ''' <summary>The Apply button.</summary>
    Private Sub btnSaveTech_Click(sender As Object, e As EventArgs) Handles btnSaveTech.Click
        SaveCurrentTech()
    End Sub

    ''' <summary>Apply the form to the current tech. Returns False (and applies nothing) if the form
    ''' has a value that can't be saved; callers that go on to write the file must stop then.</summary>
    Private Function SaveCurrentTech() As Boolean
        If CurrentTech Is Nothing Then Return True

        Try
            Dim oldId As String = CurrentTech.ID
            Dim newId As String = txtTechID.Text.Trim()

            ' Validate new ID format / uniqueness before applying.
            If String.IsNullOrEmpty(newId) Then
                MessageBox.Show("Tech ID is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTechID.Text = oldId
                Return False
            End If
            If newId <> oldId Then
                If Not System.Text.RegularExpressions.Regex.IsMatch(newId, TechConstants.TechIdPattern) Then
                    MessageBox.Show("Tech ID must be exactly 5 digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTechID.Text = oldId
                    Return False
                End If
                If TechList.ContainsKey(newId) Then
                    MessageBox.Show($"Tech ID {newId} is already in use.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTechID.Text = oldId
                    Return False
                End If
            End If

            ' A blank cost used to save as 0, which makes the tech free / pre-researched.
            Dim ec, pc As Integer
            If Not TryReadCost(txtEdenCost, "Eden", ec) Then Return False
            If Not TryReadCost(txtPlymouthCost, "Plymouth", pc) Then Return False

            ' The file format has no way to escape a double quote inside a quoted string.
            For Each field In {Tuple.Create(txtTechName, "Tech Name"), Tuple.Create(txtTeaser, "Pre-Research Description"),
                               Tuple.Create(txtDescription, "Completed Description"), Tuple.Create(txtImproveDesc, "Improve Description")}
                If field.Item1.Text.Contains("""") Then
                    MessageBox.Show($"{field.Item2} can't contain double quotes ("") because the tech tree file has no way to store them. Use single quotes (') instead.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    SelectFormField(field.Item1)
                    Return False
                End If
            Next

            CurrentTech.ID = newId
            CurrentTech.Name = txtTechName.Text.Trim()
            CurrentTech.Category = If(cmbCategory.SelectedIndex < 0, 0, cmbCategory.SelectedIndex)

            Dim ms As Integer = 0
            Integer.TryParse(txtMaxScientists.Text, ms)
            CurrentTech.MaxScientists = ms

            CurrentTech.Lab = If(cmbLab.SelectedIndex < 0, 0, cmbLab.SelectedIndex)

            CurrentTech.EdenCost = ec
            CurrentTech.PlymouthCost = pc

            ' If the user has split the costs, switch off unified-cost serialization.
            If CurrentTech.EdenCost <> CurrentTech.PlymouthCost Then
                CurrentTech.UsesUnifiedCost = False
            End If

            CurrentTech.Teaser = txtTeaser.Text
            CurrentTech.Description = txtDescription.Text
            CurrentTech.ImproveDesc = txtImproveDesc.Text

            ' Re-key the dictionary if the ID changed; update all REQUIRES references too.
            If newId <> oldId AndAlso Not String.IsNullOrEmpty(oldId) Then
                TechList.Remove(oldId)
                TechList(newId) = CurrentTech
                Dim orderIdx As Integer = TechOrder.IndexOf(oldId)
                If orderIdx >= 0 Then
                    TechOrder(orderIdx) = newId
                Else
                    TechOrder.Add(newId)
                End If
                For Each other In TechList.Values
                    For i As Integer = 0 To other.Prerequisites.Count - 1
                        If other.Prerequisites(i) = oldId Then other.Prerequisites(i) = newId
                    Next
                Next
            ElseIf Not TechList.ContainsKey(newId) Then
                TechList(newId) = CurrentTech
                If Not TechOrder.Contains(newId) Then TechOrder.Add(newId)
            End If

            ' Soft validation: warn the user, but don't reject the edit.
            Dim errs = TechValidator.ValidateTechnology(CurrentTech)
            If errs.Count > 0 Then
                Dim msg As New StringBuilder("Saved with warnings:" & vbCrLf & vbCrLf)
                For Each errMsg In errs
                    msg.AppendLine(" • " & errMsg)
                Next
                ' Use status-bar-style notification via tooltip to avoid blocking workflow.
                ToolTip1.Show(msg.ToString(), btnSaveTech, 0, -20, 3000)
            End If

            _formSnapshot = FormSnapshot()   ' the form now matches the tech
            MarkDirty()
            PopulateTechList()
            ' Restore selection on the new ID
            For i As Integer = 0 To lstTechnologies.Items.Count - 1
                If lstTechnologies.Items(i).ToString().StartsWith(newId) Then
                    lstTechnologies.SelectedIndex = i
                    Exit For
                End If
            Next
            UpdateStatusBar()
            Return True

        Catch ex As Exception
            MessageBox.Show($"Error saving technology: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>Read a cost box: must be a whole number, -1 (disabled) or more. Shows why and
    ''' selects the box if not.</summary>
    Private Function TryReadCost(box As TextBox, faction As String, ByRef value As Integer) As Boolean
        If Integer.TryParse(box.Text.Trim(), value) AndAlso value >= -1 Then Return True

        Dim problem As String = If(box.Text.Trim().Length = 0, "is empty", $"'{box.Text.Trim()}' isn't valid")
        MessageBox.Show($"{faction} Cost {problem}.{vbCrLf}{vbCrLf}Enter -1 (disabled for {faction}), 0 (free / pre-researched) or a research time greater than 0.",
            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        SelectFormField(box)
        Return False
    End Function

    ''' <summary>Bring a field on the General tab into view and select its text.</summary>
    Private Sub SelectFormField(box As TextBox)
        TabControl1.SelectedTab = TabPage1
        box.Focus()
        box.SelectAll()
    End Sub

    ' ----- Add / Duplicate / Delete -----

    ''' <summary>First unused ID in the highest tech level, for new and duplicated techs.</summary>
    Private Function SuggestNextTechId() As String
        ' Find the next free slot inside the highest existing tech level.
        If TechList.Count = 0 Then Return "02001"

        Dim maxLevel As Integer = 0
        For Each tech In TechList.Values
            Dim n As Integer
            If Integer.TryParse(tech.ID, n) Then
                Dim lvl As Integer = n \ 1000
                If lvl > maxLevel Then maxLevel = lvl
            End If
        Next

        Dim levelBase As Integer = maxLevel * 1000
        For candidate As Integer = levelBase + 1 To levelBase + 999
            Dim s As String = candidate.ToString("D5")
            If Not TechList.ContainsKey(s) Then Return s
        Next

        ' Level full; bump to next level
        Return ((maxLevel + 1) * 1000 + 1).ToString("D5")
    End Function

    ' The buttons below the form run the matching Edit menu commands.
    Private Sub btnNewTech_Click(sender As Object, e As EventArgs) Handles btnNewTech.Click
        AddTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    ''' <summary>Edit > Add Technology: a new tech with the next free ID, selected for editing.</summary>
    Private Sub AddTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddTechnologyToolStripMenuItem.Click
        Dim newTech As New Technology()
        newTech.ID = SuggestNextTechId()
        newTech.Name = "New Technology"
        newTech.EdenCost = 1000
        newTech.PlymouthCost = 1000

        TechList(newTech.ID) = newTech
        TechOrder.Add(newTech.ID)
        MarkDirty()
        PopulateTechList()

        For i As Integer = 0 To lstTechnologies.Items.Count - 1
            If lstTechnologies.Items(i).ToString().StartsWith(newTech.ID) Then
                lstTechnologies.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub btnDuplicateTech_Click(sender As Object, e As EventArgs) Handles btnDuplicateTech.Click
        DuplicateTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    ''' <summary>Edit > Duplicate Technology: copy the current tech under the next free ID.</summary>
    Private Sub DuplicateTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DuplicateTechnologyToolStripMenuItem.Click
        If CurrentTech Is Nothing Then Return

        Dim newTech As New Technology()
        newTech.Name = CurrentTech.Name & " (Copy)"
        newTech.Category = CurrentTech.Category
        newTech.EdenCost = CurrentTech.EdenCost
        newTech.PlymouthCost = CurrentTech.PlymouthCost
        newTech.MaxScientists = CurrentTech.MaxScientists
        newTech.Lab = CurrentTech.Lab
        newTech.Description = CurrentTech.Description
        newTech.Teaser = CurrentTech.Teaser
        newTech.ImproveDesc = CurrentTech.ImproveDesc
        newTech.UsesUnifiedCost = CurrentTech.UsesUnifiedCost

        For Each prereq In CurrentTech.Prerequisites
            newTech.Prerequisites.Add(prereq)
        Next
        For Each prop In CurrentTech.UnitProperties
            newTech.UnitProperties.Add(New UnitProperty() With {
                .UnitType = prop.UnitType,
                .PropertyName = prop.PropertyName,
                .Value = prop.Value
            })
        Next
        newTech.ExtraLines.AddRange(CurrentTech.ExtraLines)

        newTech.ID = SuggestNextTechId()
        TechList(newTech.ID) = newTech
        TechOrder.Add(newTech.ID)
        MarkDirty()
        PopulateTechList()

        For i As Integer = 0 To lstTechnologies.Items.Count - 1
            If lstTechnologies.Items(i).ToString().StartsWith(newTech.ID) Then
                lstTechnologies.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub btnDeleteTech_Click(sender As Object, e As EventArgs) Handles btnDeleteTech.Click
        DeleteTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    ''' <summary>Edit > Delete Technology, after confirming. Other techs' REQUIRES of it are removed too.</summary>
    Private Sub DeleteTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTechnologyToolStripMenuItem.Click
        If CurrentTech Is Nothing Then Return
        If MessageBox.Show($"Delete '{CurrentTech.Name}' (ID {CurrentTech.ID})?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim removedId As String = CurrentTech.ID
        TechList.Remove(removedId)
        TechOrder.Remove(removedId)

        ' Strip references in other techs' prereqs so the tree stays internally consistent.
        For Each other In TechList.Values
            other.Prerequisites.RemoveAll(Function(p) p = removedId)
        Next

        MarkDirty()
        PopulateTechList()
        ClearForm()
    End Sub

    ' ----- Prerequisites -----

    ''' <summary>Add the tech chosen in the drop-down as a prerequisite of the current tech.</summary>
    Private Sub btnAddPrereq_Click(sender As Object, e As EventArgs) Handles btnAddPrereq.Click
        If cmbAvailablePrereqs.SelectedIndex < 0 OrElse CurrentTech Is Nothing Then Return

        Dim selectedText As String = cmbAvailablePrereqs.SelectedItem.ToString()
        Dim prereqID As String = selectedText.Split(" "c)(0)

        If prereqID = CurrentTech.ID Then Return
        If Not CurrentTech.Prerequisites.Contains(prereqID) Then
            CurrentTech.Prerequisites.Add(prereqID)
            MarkDirty()
            RefreshTechLists()
        End If
    End Sub

    ''' <summary>Remove the selected prerequisite.</summary>
    Private Sub btnRemovePrereq_Click(sender As Object, e As EventArgs) Handles btnRemovePrereq.Click
        If lstPrerequisites.SelectedIndex < 0 OrElse CurrentTech Is Nothing Then Return

        Dim selectedText As String = lstPrerequisites.SelectedItem.ToString()
        Dim prereqID As String = selectedText.Split(" "c)(0)

        CurrentTech.Prerequisites.Remove(prereqID)
        MarkDirty()
        RefreshTechLists()
    End Sub

    ''' <summary>Double-click a prerequisite to open it; a missing one has its ID copied instead.</summary>
    Private Sub lstPrerequisites_DoubleClick(sender As Object, e As EventArgs) Handles lstPrerequisites.DoubleClick
        If lstPrerequisites.SelectedIndex < 0 Then Return
        Dim selectedText As String = lstPrerequisites.SelectedItem.ToString()
        Dim techID As String = selectedText.Split(" "c)(0)

        ' Jump to the referenced tech instead of just copying its ID.
        If TechList.ContainsKey(techID) Then
            For i As Integer = 0 To lstTechnologies.Items.Count - 1
                If lstTechnologies.Items(i).ToString().StartsWith(techID) Then
                    lstTechnologies.SelectedIndex = i
                    Return
                End If
            Next
        End If

        Clipboard.SetText(techID)
        ToolTip1.Show($"Copied: {techID}", lstPrerequisites, 0, -20, 1000)
    End Sub

    ' ----- Unit Properties -----

    ''' <summary>Add a UNIT_PROP from the three fields; the value must be a whole number.</summary>
    Private Sub btnAddUnitProp_Click(sender As Object, e As EventArgs) Handles btnAddUnitProp.Click
        If CurrentTech Is Nothing OrElse cmbUnitType.SelectedIndex < 0 OrElse
           cmbUnitProperty.SelectedIndex < 0 OrElse String.IsNullOrEmpty(txtUnitValue.Text) Then
            MessageBox.Show("Please fill in all unit property fields.", "Missing Information",
                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim v As Integer
        If Not Integer.TryParse(txtUnitValue.Text.Trim(), v) Then
            MessageBox.Show("Unit property value must be a whole number.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        CurrentTech.UnitProperties.Add(New UnitProperty() With {
            .UnitType = cmbUnitType.SelectedItem.ToString(),
            .PropertyName = cmbUnitProperty.SelectedItem.ToString(),
            .Value = v.ToString()
        })

        cmbUnitType.SelectedIndex = -1
        cmbUnitProperty.SelectedIndex = -1
        txtUnitValue.Text = ""

        MarkDirty()
        RefreshTechLists()
    End Sub

    ''' <summary>Remove the selected unit property.</summary>
    Private Sub btnRemoveUnitProp_Click(sender As Object, e As EventArgs) Handles btnRemoveUnitProp.Click
        If lstUnitProperties.SelectedIndices.Count = 0 OrElse CurrentTech Is Nothing Then Return
        Dim index As Integer = lstUnitProperties.SelectedIndices(0)
        CurrentTech.UnitProperties.RemoveAt(index)
        MarkDirty()
        RefreshTechLists()
    End Sub

    ' ----- Search / Filter -----

    ' Any change to the search text or category filter refilters the list.
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        PopulateTechList()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        PopulateTechList()
    End Sub

    Private Sub cmbCategoryFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategoryFilter.SelectedIndexChanged
        PopulateTechList()
    End Sub

    ' ----- Cost ↔ Checkbox two-way binding -----

    ' The Eden / Plymouth / Both boxes and the two cost boxes mirror each other: unticking a
    ' faction sets its cost to -1, ticking it restores a usable cost, and typing a cost updates
    ' the ticks. IsLoading stops each handler from re-triggering the others.
    Private Sub chkEden_CheckedChanged(sender As Object, e As EventArgs) Handles chkEden.CheckedChanged
        If IsLoading Then Return
        IsLoading = True
        Try
            If chkEden.Checked Then
                Dim v As Integer
                If Not Integer.TryParse(txtEdenCost.Text, v) OrElse v < 0 Then
                    txtEdenCost.Text = "1000"
                End If
            Else
                txtEdenCost.Text = "-1"
            End If
            chkBoth.Checked = chkEden.Checked AndAlso chkPlymouth.Checked
        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub chkPlymouth_CheckedChanged(sender As Object, e As EventArgs) Handles chkPlymouth.CheckedChanged
        If IsLoading Then Return
        IsLoading = True
        Try
            If chkPlymouth.Checked Then
                Dim v As Integer
                If Not Integer.TryParse(txtPlymouthCost.Text, v) OrElse v < 0 Then
                    txtPlymouthCost.Text = "1000"
                End If
            Else
                txtPlymouthCost.Text = "-1"
            End If
            chkBoth.Checked = chkEden.Checked AndAlso chkPlymouth.Checked
        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub chkBoth_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoth.CheckedChanged
        If IsLoading Then Return
        IsLoading = True
        Try
            If chkBoth.Checked Then
                chkEden.Checked = True
                chkPlymouth.Checked = True
                Dim v As Integer
                If Not Integer.TryParse(txtEdenCost.Text, v) OrElse v < 0 Then txtEdenCost.Text = "1000"
                If Not Integer.TryParse(txtPlymouthCost.Text, v) OrElse v < 0 Then txtPlymouthCost.Text = "1000"
            Else
                ' Unchecking "Both" disables both factions (would otherwise be a no-op).
                chkEden.Checked = False
                chkPlymouth.Checked = False
                txtEdenCost.Text = "-1"
                txtPlymouthCost.Text = "-1"
            End If
        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub txtEdenCost_TextChanged(sender As Object, e As EventArgs) Handles txtEdenCost.TextChanged
        If IsLoading Then Return
        IsLoading = True
        Try
            Dim v As Integer
            If Integer.TryParse(txtEdenCost.Text, v) Then
                chkEden.Checked = (v >= 0)
                chkBoth.Checked = chkEden.Checked AndAlso chkPlymouth.Checked
            End If
        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub txtPlymouthCost_TextChanged(sender As Object, e As EventArgs) Handles txtPlymouthCost.TextChanged
        If IsLoading Then Return
        IsLoading = True
        Try
            Dim v As Integer
            If Integer.TryParse(txtPlymouthCost.Text, v) Then
                chkPlymouth.Checked = (v >= 0)
                chkBoth.Checked = chkEden.Checked AndAlso chkPlymouth.Checked
            End If
        Finally
            IsLoading = False
        End Try
    End Sub

    ' Restrict numeric fields to integer input (allowing minus and backspace).
    Private Sub IntegerKeyPress(sender As Object, e As KeyPressEventArgs) Handles _
        txtEdenCost.KeyPress, txtPlymouthCost.KeyPress,
        txtMaxScientists.KeyPress, txtUnitValue.KeyPress

        Dim tb As TextBox = TryCast(sender, TextBox)
        If tb Is Nothing Then Return

        If Char.IsControl(e.KeyChar) Then Return
        If Char.IsDigit(e.KeyChar) Then Return
        ' Minus only valid as first character
        If e.KeyChar = "-"c AndAlso tb.SelectionStart = 0 AndAlso Not tb.Text.StartsWith("-") Then Return

        e.Handled = True
    End Sub

    ' ----- Tech ID change → trigger re-key on losing focus (so prereq combos refresh) -----
    Private Sub txtTechID_TextChanged(sender As Object, e As EventArgs) Handles txtTechID.TextChanged
        ' Intentionally no-op; re-key happens in SaveCurrentTech.
    End Sub

    ''' <summary>Show the icon for the chosen category.</summary>
    Private Sub cmbCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategory.SelectedIndexChanged
        If Not IsLoading Then UpdateCategoryIcon()
    End Sub

    ''' <summary>Category icon next to the name: the in-game research icon (none for category 0).</summary>
    Private Sub UpdateCategoryIcon()
        Try
            pnlResearchIcon.BackgroundImage = Nothing
            Select Case cmbCategory.SelectedIndex
                Case 1 : pnlResearchIcon.BackgroundImage = My.Resources.icon1
                Case 2 : pnlResearchIcon.BackgroundImage = My.Resources.icon2
                Case 3 : pnlResearchIcon.BackgroundImage = My.Resources.icon3
                Case 4 : pnlResearchIcon.BackgroundImage = My.Resources.icon4
                Case 5 : pnlResearchIcon.BackgroundImage = My.Resources.icon5
                Case 6 : pnlResearchIcon.BackgroundImage = My.Resources.icon6
                Case 7 : pnlResearchIcon.BackgroundImage = My.Resources.icon7
                Case 8 : pnlResearchIcon.BackgroundImage = My.Resources.icon8
                Case 9 : pnlResearchIcon.BackgroundImage = My.Resources.icon9
                Case 10 : pnlResearchIcon.BackgroundImage = My.Resources.icon10
                Case 11 : pnlResearchIcon.BackgroundImage = My.Resources.icon11
                Case 12 : pnlResearchIcon.BackgroundImage = My.Resources.icon12
            End Select
        Catch
            pnlResearchIcon.BackgroundImage = Nothing
        End Try
    End Sub

    ' Form fields are a scratch pad: only "Apply Changes" (SaveCurrentTech, which File > Save also
    ' runs) commits them to the in-memory tree and marks it dirty. Add/Delete/Duplicate and
    ' prereq/unit-prop edits mark dirty directly since they bypass the form.

    ' ----- Help / About / Close -----

    ''' <summary>Short usage notes.</summary>
    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        MessageBox.Show("Outpost 2 Tech Tree Editor" & vbCrLf & vbCrLf &
            "- File > Open: load an existing tech tree (.txt)" & vbCrLf &
            "- File > Save: persist changes to disk" & vbCrLf &
            "- 'Apply Changes' on the form commits edits to the in-memory tree only." & vbCrLf &
            "- Use -1 cost to make a tech unavailable to a faction" & vbCrLf &
            "- Changing a Tech ID updates all REQUIRES references" & vbCrLf &
            "- Right-click the tech list to Find References or View Raw File" & vbCrLf &
            "- Tools > View Tech Tree in Browser (F5) shows the whole tree Civilization-style" & vbCrLf & vbCrLf &
            "Tech ID format: XXYYZ where (XX * 1000) is the tech level.",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Help > About.</summary>
    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Using AboutForm As New fAbout()
            AboutForm.ApplicationTitle = ApplicationName
            AboutForm.ApplicationVersion = ApplicationVersion
            AboutForm.ApplicationBuild = ApplicationBuild
            AboutForm.ApplicationDescription = ApplicationDescription
            AboutForm.ApplicationCopyright = "Outpost Universe © 2025-2026"
            AboutForm.ApplicationWebsite = ApplicationWebsite
            AboutForm.ShowDialog(Me)
        End Using
    End Sub

    ''' <summary>File > Exit. FormClosing offers to save first.</summary>
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    ''' <summary>Offer to save unsaved changes; Cancel keeps the window open.</summary>
    Private Sub fMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not PromptSaveChanges() Then e.Cancel = True
    End Sub

    ' ----- Validation / Stats / Dependencies -----

    ''' <summary>Tools > Validate Current Tech: check the current tech against the format rules.</summary>
    Private Sub ValidateCurrentTech()
        If CurrentTech Is Nothing Then Return
        Dim errors = TechValidator.ValidateTechnology(CurrentTech)
        If errors.Count > 0 Then
            Dim msg As New StringBuilder("Validation Errors:" & vbCrLf & vbCrLf)
            For Each errMsg In errors
                msg.AppendLine(" • " & errMsg)
            Next
            MessageBox.Show(msg.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show("Technology validation passed!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ''' <summary>Tools > Validate Tech Tree: missing prerequisites, circular dependencies, limits.</summary>
    Private Sub ValidateTechTree()
        Dim errors = TechValidator.ValidateTechTree(TechList)
        If errors.Count > 0 Then
            Dim msg As New StringBuilder("Tech Tree Validation Errors:" & vbCrLf & vbCrLf)
            For Each errMsg In errors
                msg.AppendLine(" • " & errMsg)
            Next
            MessageBox.Show(msg.ToString(), "Tech Tree Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show("Tech tree validation passed!", "Tech Tree Validation", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ''' <summary>Tools > Statistics: tech counts per faction and category, total research cost.</summary>
    Private Sub ShowStatistics()
        Dim stats = TechImportExport.GetTechStatistics(TechList)
        Dim sb As New StringBuilder()
        sb.AppendLine("Tech Tree Statistics")
        sb.AppendLine("===================")
        sb.AppendLine()
        sb.AppendLine($"Total Technologies: {stats("TotalTechnologies")}")
        sb.AppendLine($"Eden Only: {stats("EdenOnlyTechs")}")
        sb.AppendLine($"Plymouth Only: {stats("PlymouthOnlyTechs")}")
        sb.AppendLine($"Both Factions: {stats("BothFactionTechs")}")
        sb.AppendLine($"Free Technologies: {stats("FreeTechs")}")
        sb.AppendLine($"Researchable Technologies: {stats("ResearchableTechs")}")
        sb.AppendLine()
        sb.AppendLine($"Total Eden Research Cost: {CInt(stats("TotalEdenCost")):N0} RP")
        sb.AppendLine($"Total Plymouth Research Cost: {CInt(stats("TotalPlymouthCost")):N0} RP")
        sb.AppendLine()
        sb.AppendLine("Technologies by Category:")
        Dim cb = DirectCast(stats("CategoryBreakdown"), Dictionary(Of Integer, Integer))
        For Each kvp In cb.OrderBy(Function(x) x.Key)
            sb.AppendLine($"  {kvp.Key.GetCategoryName()}: {kvp.Value}")
        Next
        MessageBox.Show(sb.ToString(), "Tech Tree Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Display text of every tech that requires `techId`.</summary>
    Private Function FindReferencingTechs(techId As String) As List(Of String)
        Dim references As New List(Of String)
        For Each tech In TechList.Values
            If tech.Prerequisites.Contains(techId) Then references.Add(tech.GetDisplayText())
        Next
        Return references
    End Function

    ''' <summary>Tools > Show Dependencies: open the Dependencies tab, or say there are none.</summary>
    Private Sub ShowDependencyTree()
        If CurrentTech Is Nothing Then Return

        If TabControl1.TabPages.Contains(TabPage3) Then
            TabControl1.SelectedTab = TabPage3
        Else
            MessageBox.Show($"{CurrentTech.Name} has no prerequisites and no technology requires it.",
                "Dependencies", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' ----- Dependencies tab -----
    ' Shown only for techs with prerequisites or dependents. Both trees load one level at a time
    ' (on expand), since the full "required by" tree of an early campaign tech repeats most of the file.

    ''' <summary>Techs that list `techId` as a prerequisite, by ID.</summary>
    Private Function GetDependents(techId As String) As List(Of Technology)
        Return TechList.Values.Where(Function(t) t.Prerequisites.Contains(techId)).OrderBy(Function(t) t.ID).ToList()
    End Function

    ''' <summary>IDs one step away: prerequisites (upstream) or dependents (downstream).</summary>
    Private Function GetDependencyIds(techId As String, upstream As Boolean) As List(Of String)
        If upstream Then
            Return If(TechList.ContainsKey(techId), TechList(techId).Prerequisites.ToList(), New List(Of String))
        End If
        Return GetDependents(techId).Select(Function(t) t.ID).ToList()
    End Function

    ''' <summary>Number of distinct techs in the full chain above (upstream) or below the given tech.</summary>
    Private Function CountDependencyChain(techId As String, upstream As Boolean) As Integer
        Dim seen As New HashSet(Of String)
        Dim pending As New Stack(Of String)
        pending.Push(techId)
        While pending.Count > 0
            For Each nextId In GetDependencyIds(pending.Pop(), upstream)
                If nextId <> techId AndAlso seen.Add(nextId) Then pending.Push(nextId)
            Next
        End While
        Return seen.Count
    End Function

    ''' <summary>Rebuild the Dependencies tab for the current tech, or hide it when the tech has no dependencies.</summary>
    Private Sub UpdateDependenciesTab()
        tvRequires.BeginUpdate()
        tvRequiredBy.BeginUpdate()
        Try
            tvRequires.Nodes.Clear()
            tvRequiredBy.Nodes.Clear()

            Dim prereqIds = If(CurrentTech Is Nothing, New List(Of String), CurrentTech.Prerequisites.ToList())
            Dim dependentIds = If(CurrentTech Is Nothing, New List(Of String), GetDependencyIds(CurrentTech.ID, upstream:=False))

            If prereqIds.Count = 0 AndAlso dependentIds.Count = 0 Then
                TabControl1.TabPages.Remove(TabPage3)
                Return
            End If
            If Not TabControl1.TabPages.Contains(TabPage3) Then TabControl1.TabPages.Add(TabPage3)

            TabPage3.Text = $"Dependencies ({prereqIds.Count + dependentIds.Count})"
            FillDependencyTree(tvRequires, grpRequires, "Requires", prereqIds, upstream:=True)
            FillDependencyTree(tvRequiredBy, grpRequiredBy, "Required by", dependentIds, upstream:=False)
        Finally
            tvRequires.EndUpdate()
            tvRequiredBy.EndUpdate()
        End Try
    End Sub

    ''' <summary>Fill one tree with its first level and set the group title with the counts.</summary>
    Private Sub FillDependencyTree(tree As TreeView, group As GroupBox, title As String,
                                   ids As List(Of String), upstream As Boolean)
        If ids.Count = 0 Then
            group.Text = title
            tree.Nodes.Add(New TreeNode("(none)") With {.ForeColor = SystemColors.GrayText})
            Return
        End If

        Dim total As Integer = CountDependencyChain(CurrentTech.ID, upstream)
        group.Text = If(total > ids.Count, $"{title} ({ids.Count} direct, {total} in full chain)", $"{title} ({ids.Count})")
        For Each id In ids
            tree.Nodes.Add(CreateDependencyNode(id, upstream, New HashSet(Of String) From {CurrentTech.ID}))
        Next
    End Sub

    ''' <summary>Tree node for a tech. Its children are added on first expand; until then a
    ''' placeholder child (Tag = Nothing) gives it an expand arrow.</summary>
    Private Function CreateDependencyNode(techId As String, upstream As Boolean, path As HashSet(Of String)) As TreeNode
        Dim node As New TreeNode() With {.Tag = techId}
        If Not TechList.ContainsKey(techId) Then
            node.Text = techId & " (Missing!)"
            node.ForeColor = Color.Firebrick
        ElseIf path.Contains(techId) Then
            node.Text = TechList(techId).GetDisplayText() & " (circular!)"
            node.ForeColor = Color.Firebrick
        Else
            node.Text = TechList(techId).GetDisplayText()
            If GetDependencyIds(techId, upstream).Count > 0 Then node.Nodes.Add(New TreeNode("..."))
        End If
        Return node
    End Function

    ''' <summary>Load a node's children the first time it is expanded.</summary>
    Private Sub DependencyTree_BeforeExpand(sender As Object, e As TreeViewCancelEventArgs) Handles tvRequires.BeforeExpand, tvRequiredBy.BeforeExpand
        If e.Node.Nodes.Count <> 1 OrElse e.Node.Nodes(0).Tag IsNot Nothing Then Return   ' already loaded

        Dim upstream As Boolean = e.Node.TreeView Is tvRequires
        ' Techs on the way from the current tech down to this node, for cycle detection.
        Dim path As New HashSet(Of String) From {CurrentTech.ID}
        Dim walk As TreeNode = e.Node
        While walk IsNot Nothing
            path.Add(CStr(walk.Tag))
            walk = walk.Parent
        End While

        e.Node.Nodes.Clear()
        For Each id In GetDependencyIds(CStr(e.Node.Tag), upstream)
            e.Node.Nodes.Add(CreateDependencyNode(id, upstream, path))
        Next
    End Sub

    ''' <summary>Double-click a tech in either tree to open it.</summary>
    Private Sub DependencyTree_NodeMouseDoubleClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvRequires.NodeMouseDoubleClick, tvRequiredBy.NodeMouseDoubleClick
        Dim techId As String = TryCast(e.Node.Tag, String)
        ' Deferred: selecting rebuilds both trees, which must not happen inside the TreeView's own click handling.
        If techId IsNot Nothing AndAlso TechList.ContainsKey(techId) Then BeginInvoke(Sub() SelectTechInList(techId))
    End Sub

    ''' <summary>Select a tech in the list, clearing the search/category filter if it is hidden.</summary>
    Private Sub SelectTechInList(techId As String)
        For pass As Integer = 0 To 1
            For i As Integer = 0 To lstTechnologies.Items.Count - 1
                If lstTechnologies.Items(i).ToString().StartsWith(techId & " ") Then
                    lstTechnologies.SelectedIndex = i
                    Return
                End If
            Next
            txtSearch.Text = ""
            cmbCategoryFilter.SelectedIndex = 0
        Next
    End Sub

    ''' <summary>Tools > Export to CSV.</summary>
    Private Sub ExportToCSV()
        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
        saveDialog.Title = "Export Tech Tree to CSV"
        saveDialog.FileName = "techTree.csv"

        If saveDialog.ShowDialog() <> DialogResult.OK Then Return
        Try
            TechImportExport.ExportToCSV(TechList, saveDialog.FileName)
            MessageBox.Show($"Tech tree exported to {Path.GetFileName(saveDialog.FileName)}.",
                "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Export Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ----- Civilization-style tech tree view (HTML) -----

    ''' <summary>Name used for the HTML view and export: the file name, or "New File".</summary>
    Private Function TechTreeTitle() As String
        Return If(String.IsNullOrEmpty(FilePath), "New File", Path.GetFileName(FilePath))
    End Function

    ''' <summary>Render the in-memory tree (including unsaved edits) to a temp HTML file and open it in the default browser.</summary>
    Private Sub ViewTechTreeInBrowser()
        If TechList.Count = 0 Then
            MessageBox.Show("There are no technologies to show. Open or create a tech tree first.", "View Tech Tree",
                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Try
            Dim fileName As String = Path.Combine(Path.GetTempPath(), ApplicationName,
                Path.GetFileNameWithoutExtension(TechTreeTitle()) & "_techtree.html")
            TechTreeHtmlExporter.Export(TechList, TechOrder, TechTreeTitle(), fileName)
            Process.Start(New ProcessStartInfo(fileName) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show($"Error opening tech tree view: {ex.Message}", "View Tech Tree",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Tools > Export Tech Tree to HTML: save the page where the user chooses (default: next to the .txt).</summary>
    Private Sub ExportToHTML()
        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = "HTML Files (*.html)|*.html|All Files (*.*)|*.*"
            saveDialog.Title = "Export Tech Tree to HTML"
            saveDialog.FileName = Path.GetFileNameWithoutExtension(TechTreeTitle()) & "_techtree.html"
            ' Default to the tech file's own folder so the HTML copy sits next to the .txt.
            If Not String.IsNullOrEmpty(FilePath) Then saveDialog.InitialDirectory = Path.GetDirectoryName(FilePath)

            If saveDialog.ShowDialog() <> DialogResult.OK Then Return
            Try
                TechTreeHtmlExporter.Export(TechList, TechOrder, TechTreeTitle(), saveDialog.FileName)
                If MessageBox.Show($"Tech tree exported to {Path.GetFileName(saveDialog.FileName)}.{vbCrLf}{vbCrLf}Open it now?",
                        "Export Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                    Process.Start(New ProcessStartInfo(saveDialog.FileName) With {.UseShellExecute = True})
                End If
            Catch ex As Exception
                MessageBox.Show($"Error exporting to HTML: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    ''' <summary>Tools > Create Sample: replace the tree with a small example (tech levels 1-5 and one sample tech).</summary>
    Private Sub CreateSampleTechTree()
        If MessageBox.Show("This will replace the current tech tree with a sample. Continue?",
                "Create Sample", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        If Not PromptSaveChanges() Then Return

        TechList = TechFileGenerator.CreateBasicTechTree()
        TechOrder = TechList.Keys.OrderBy(Function(k) k).ToList()
        HeaderLines.Clear()
        FooterLines.Clear()
        TrailerPrefix = Nothing
        TrailerBytes = Nothing
        FilePath = ""
        ClearForm()
        MarkDirty()
        UpdateTitle()
        PopulateTechList()
        UpdateStatusBar()
        MessageBox.Show("Sample tech tree created!", "Sample Created", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' ----- Context menu actions -----

    ''' <summary>Tech list right-click > Find References: techs that require the selected tech.</summary>
    Private Sub TechListFindReferences_Click(sender As Object, e As EventArgs)
        If lstTechnologies.SelectedIndex < 0 Then Return
        Dim selectedText As String = lstTechnologies.SelectedItem.ToString()
        Dim techID As String = selectedText.Split(" "c)(0)
        Dim refs = FindReferencingTechs(techID)

        Dim sb As New StringBuilder()
        sb.AppendLine($"Techs that require {techID}:")
        sb.AppendLine()
        If refs.Count = 0 Then
            sb.AppendLine("  (none)")
        Else
            For Each r In refs
                sb.AppendLine("  • " & r)
            Next
        End If
        MessageBox.Show(sb.ToString(), "Find References", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Tech list right-click > View Raw File: the text File > Save would write.</summary>
    Private Sub ViewRawFile_Click(sender As Object, e As EventArgs)
        Dim rendered As String = RenderFileText()
        If TrailerPrefix IsNot Nothing Then
            rendered &= TrailerPrefix & "[" & (If(TrailerBytes Is Nothing, 0, TrailerBytes.Length)) & " trailing binary bytes preserved]"
        End If
        ShowTextDialog("Raw file preview (what Save will write)", rendered)
    End Sub

    ''' <summary>Read-only text window, used for the raw file preview.</summary>
    Private Sub ShowTextDialog(title As String, content As String)
        Dim f As New Form()
        f.Text = title
        f.Icon = Me.Icon
        f.Width = 900
        f.Height = 700
        f.StartPosition = FormStartPosition.CenterParent

        Dim tb As New TextBox()
        tb.Multiline = True
        tb.ReadOnly = True
        tb.ScrollBars = ScrollBars.Both
        tb.WordWrap = False
        tb.Dock = DockStyle.Fill
        tb.Font = New System.Drawing.Font("Consolas", 9)
        tb.Text = content
        f.Controls.Add(tb)
        f.ShowDialog(Me)
    End Sub

    ' ----- Menu wiring -----

    Private Sub ValidateCurrentTechToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ValidateCurrentTechToolStripMenuItem.Click
        ValidateCurrentTech()
    End Sub

    Private Sub ValidateTechTreeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ValidateTechTreeToolStripMenuItem.Click
        ValidateTechTree()
    End Sub

    Private Sub StatisticsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StatisticsToolStripMenuItem.Click
        ShowStatistics()
    End Sub

    Private Sub ExportToCSVToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToCSVToolStripMenuItem.Click
        ExportToCSV()
    End Sub

    Private Sub ViewTechTreeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewTechTreeToolStripMenuItem.Click
        ViewTechTreeInBrowser()
    End Sub

    Private Sub ExportToHTMLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToHTMLToolStripMenuItem.Click
        ExportToHTML()
    End Sub

    Private Sub CreateSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateSampleToolStripMenuItem.Click
        CreateSampleTechTree()
    End Sub

    Private Sub ShowDependenciesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowDependenciesToolStripMenuItem.Click
        ShowDependencyTree()
    End Sub

    Private Sub cmbUnitType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUnitType.SelectedIndexChanged
        ' Hook for future per-unit-type filtering of valid properties; intentionally empty.
    End Sub

End Class
