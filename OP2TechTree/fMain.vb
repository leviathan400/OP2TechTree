Imports System.IO
Imports System.Media
Imports System.Reflection.Emit
Imports System.Text

' OP2TechTree
' https://github.com/leviathan400/OP2TechTree
'
' Create and modify Outpost 2 tech trees
' Default tech trees: edentek.txt, ply_tek.txt, tutortek.txt, multitek.txt
'
'
' Outpost 2: Divided Destiny is a real-time strategy video game released in 1997.


' Some notes from Hooman: https://forum.outpost2.net/index.php/topic,5766
' - There Is a max of 256 techs. (Static buffer limit, unchecked)
' - There Is a per tech max Of 32 REQUIRED_TECH tags. (Static buffer limit, unchecked)
' - There Is a per tech max Of 32 upgrade tags (UNIT_PROP, PLAYER_PROP, FUNCTION_RESULT). (Static buffer limit, unchecked)
' - Quoted strings have embedded newline And tab characters stripped. (Allows formatting In tech file)
' - Quoted strings must fit within a tag specific Static buffer limit. (Excludes tabs And newlines, String will be null terminated).
' - Tech name must Not exceed 100 characters. (Static buffer size 101 bytes)
' - Tech teaser, description, and improvement description must Not exceed 800 characters. (Static buffer size 801 bytes)
' - Numbers can take an Optional leading minus sign. (No positive sign allowed)
' - Numbers must fit In a Static buffer Of 32 bytes, including the leading sign. (Static buffer, checked, Parse Error)
' - Numbers must be representable As a 32-bit signed Integer. (Unchecked, Undefined behaviour)
' - The techLevel = techNum / 1000, must Not be 0. (Checked, Parse Error)
' - The techId(And techLevel) should Not be negative. (Unexpected values)
' - The armour(level) Property must be Set from 0..5 (inclusive). (Unchecked, Undefined behaviour)
' - The game supports 4..18 (inclusive) max scientists on a research topic. (Unchecked, Undefined behaiour)
' - The game sets MAX_SCIENTISTS To 1 For the Tiger speed modification In multitek.txt. (Avoids undefined behaviour by Not researching In a lab)
' - The game sets MAX_SCIENTISTS To 1 For Children's Module and Evacation Module in edentek.txt and ply_tek.txt. (Avoids undefined behaviour by not researching in a lab)
' - The game sets MAX_SCIENTISTS To 1 For 5 techs In tutortek.txt. (Avoids undefined behaviour by Not researching In a lab)
' - The lab value should only be Set To 1 = BasicLab, 2 = StandardLab, 3 = AdvancedLab. (Unchecked, Unexpected values)
' - The lab value Is typically Not Set For free And disabled techs. (Convention For built In techs)
' - Either COST Is present, Or both EDEN_COST And PLYMOUTH_COST are present. (Unchecked, possibly undefined behaviour)
' - The COST values should either be -1 (disabled), 0 (free), Or a positive value. (Unchecked, possibly undefined behaviour)
' - Each BEGIN_TECH must be paired with END_TECH. All other tags are optional. (Checked, Parse error)
' - Single value tags will overwrite their previous values If used more than once. (Unchecked, excludes REQUIRES, UNIT_PROP, PLAYER_PROP, And FUNCTION_RESULT)
' - Every tech typically specifies at least CATEGORY And COST. (Convention For built In techs)
' - The REQUIRES tag should specify a valid techID. (Unchecked, possibly undefined behaviour)
' - The PLAYER_PROP, and FUNCTION_RESULT tags are Not used by the standard tech tree files, and are basically useless.

' Tech Format:
' BEGIN_TECH      string:techName    int:techId
' CATEGORY        int:category
' DESCRIPTION     string:description
' TEASER          string:teaser
' IMPROVE_DESC    string:improveDesc
' REQUIRES        int:requiredTechId
' COST            int:cost    [edenCost = plymouthCost]
' EDEN_COST       int:cost
' PLYMOUTH_COST   int:cost
' MAX_SCIENTISTS  int:maxScientists  [4..18]
' LAB             int:lab     [1 = Basic, 2 = Standard, 3 = Advanced]
' UNIT_PROP       token:unitTypeName    token:propName    int:newPropValue
' END_TECH

'UNIT_PROP:
' Hit_Points            [Units, but generally Vehicles]
' Armor                 [Units, but specifically: GuardTower, Lynx, Panther]
' Production_Capactity  [Buildings, both factory build rates, And capacity For others]
' Move_Speed            [Vehicles]
' Sight_Range           [Units, but specifically: LightTower, GuardTower, Weapons]
' Rate_Of_Fire          [Weapons]
' Build_Time            [Not used In standard tech files]
' Power_Required        [Buildings, but specifically: GORF, Smelter, Rare Smelter]
' Storage_Capacity      [Specifically: University train rate]
' Storage_Bays          [Not used In standard tech files]
' Production_Rate       [Vehicles, mostly colony building And repair]
' Concussion_Damage     [Weapons]
' Penetration_Damage    [Weapons]
' Improved              [Unit specific: Mine, RareMine, Smelter, RareSmelter, GORF, Observatory]
' Workers_Required      [Buildings, but specifically: DIRT]
' Common_Required       [Units, but generally vehicles]
' Rare_Required         [Units, but generally vehicles]
' Turn_Rate             [Not used In standard tech files]
' Build_Points          [Not used In standard tech files]

Public Class fMain

    Public ApplicationName As String = "OP2TechTree"
    Public Version As String = "0.5.0"

    ' Data structures
    Private TechList As New Dictionary(Of String, Technology)
    Private CurrentTech As Technology
    Private FilePath As String = ""
    Private IsLoading As Boolean = False

    ' Technology class to hold all tech data
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
        Public TechIcon As String = ""

        Public ReadOnly Property IsEdenAvailable As Boolean
            Get
                Return EdenCost >= 0  ' Available if >= 0 (includes free techs)
            End Get
        End Property

        Public ReadOnly Property IsPlymouthAvailable As Boolean
            Get
                Return PlymouthCost >= 0  ' Available if >= 0 (includes free techs)
            End Get
        End Property

        Public ReadOnly Property IsBothFactions As Boolean
            Get
                Return IsEdenAvailable AndAlso IsPlymouthAvailable
            End Get
        End Property

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

    Public Class UnitProperty
        Public UnitType As String = ""
        Public PropertyName As String = ""
        Public Value As String = ""
    End Class

    Private Sub fMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.WriteLine("--- " & ApplicationName & " Started ---")
        Me.Icon = My.Resources.testplans

        InitializeComboBoxes()
        ClearForm()
        UpdateStatusBar()

        Dim player As New SoundPlayer(My.Resources.lab_3)
        player.Play()
    End Sub

    Private Sub InitializeComboBoxes()
        ' Initialize category combo boxes
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

        cmbCategory.Items.AddRange(categories)
        cmbCategoryFilter.Items.Add("All Categories")
        cmbCategoryFilter.Items.AddRange(categories)
        cmbCategoryFilter.SelectedIndex = 0

        ' Initialize lab combo box
        cmbLab.Items.AddRange({"None", "1 - Basic Lab", "2 - Standard Lab", "3 - Advanced Lab"})

        ' Not a complete list of units ?? (vech, buildings, weapons)
        ' 68 in the list:
        ' 15 Vehicles
        ' 39 Buildings
        ' 14 Weapons inc EMP missile

        ' Initialize unit type combo box
        Dim unitTypes() As String = {
            "ACID", "AGRIDOME", "ASE", "BIG_TANK", "BIG_TRUCK", "BULL_DOZER",
            "CANNON", "COMMAND", "CON_TRUCK", "DIRT", "EMP", "EMP_MISSILE",
            "FACT_ANDROID", "FACT_LUXURY", "FACT_STRUCTURE", "FACT_VEHICLE",
            "FOAM", "FORUM", "GARAGE", "GEO_METRO", "GEOTHERMAL", "GORF",
            "LAB", "LAB_ADV", "LAB_STANDARD", "LASER", "LIGHT_TANK", "MAGMA_WELL",
            "MED_CENTER", "MED_TANK", "METEOR_DEF", "MICROWAVE", "MINE", "MINE_ADV",
            "MINER", "MOBILE_HOME", "NURSERY", "OBSERVATORY", "RAIL_GUN", "RECREATION",
            "REPAIR_TRUCK", "RESIDENCE", "RESIDENCE_E", "RESIDENCE_P", "ROBOT_COMMAND",
            "SCORPION", "SCORPION_WEAPON", "SCOUT", "SELF_DESTRUCT", "SELF_DESTRUCT_ADV",
            "SMELTER", "SMELTER_ADV", "SOLAR", "SPAM", "SPACEPORT", "SPIDER",
            "STORAGE_ORE", "STORAGE_RARE", "SURVEYOR", "THORS_HAMMER", "TOKAMAK",
            "TOWER_GUARD", "TOWER_LIGHT", "TRADE", "TUBE", "UNIVERSITY", "WALL_TRUCK"
        }
        cmbUnitType.Items.AddRange(unitTypes)

        'UNIT_PROP - 19 Property Tokens:
        ' Hit_Points            [Units, but generally Vehicles]
        ' Armor                 [Units, but specifically: GuardTower, Lynx, Panther]
        ' Production_Capactity  [Buildings, both factory build rates, And capacity For others]
        ' Move_Speed            [Vehicles]
        ' Sight_Range           [Units, but specifically: LightTower, GuardTower, Weapons]
        ' Rate_Of_Fire          [Weapons]
        ' Build_Time            [Not used In standard tech files]
        ' Power_Required        [Buildings, but specifically: GORF, Smelter, Rare Smelter]
        ' Storage_Capacity      [Specifically: University train rate]
        ' Storage_Bays          [Not used In standard tech files]
        ' Production_Rate       [Vehicles, mostly colony building And repair]
        ' Concussion_Damage     [Weapons]
        ' Penetration_Damage    [Weapons]
        ' Improved              [Unit specific: Mine, RareMine, Smelter, RareSmelter, GORF, Observatory]
        ' Workers_Required      [Buildings, but specifically: DIRT]
        ' Common_Required       [Units, but generally vehicles]
        ' Rare_Required         [Units, but generally vehicles]
        ' Turn_Rate             [Not used In standard tech files]
        ' Build_Points          [Not used In standard tech files]

        ' Add Property types
        cmbUnitProperty.Items.Clear()
        cmbUnitProperty.Items.AddRange({
            "Armor",
            "Common_Required",
            "Concussion_Damage",
            "Hit_Points",
            "Improved",
            "Move_Speed",
            "Penetration_Damage",
            "Power_Required",
            "Production_Capacity",
            "Production_Rate",
            "Rate_Of_Fire",
            "Rare_Required",
            "Sight_Range",
            "Storage_Capacity",
            "Workers_Required"
        })
    End Sub

    Private Sub ClearForm()
        ' Clear / Reset Form

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

        IsLoading = False
    End Sub

    ' File Operations
    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        If PromptSaveChanges() Then
            TechList.Clear()
            FilePath = ""
            Me.Text = "Outpost 2 Tech Tree Editor - New File"
            PopulateTechList()
            ClearForm()
        End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If PromptSaveChanges() Then
            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                LoadTechFile(OpenFileDialog1.FileName)
            End If
        End If
        ClearForm()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        SaveCurrentTech()

        If String.IsNullOrEmpty(FilePath) Then
            SaveAsToolStripMenuItem_Click(sender, e)
        Else
            SaveTechFile(FilePath)
        End If
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        SaveCurrentTech()

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            SaveTechFile(SaveFileDialog1.FileName)
            FilePath = SaveFileDialog1.FileName
            Me.Text = $"Outpost 2 Tech Tree Editor - {Path.GetFileName(FilePath)}"
        End If
    End Sub

    Private Sub LoadTechFile(fileName As String)
        Try
            TechList.Clear()
            Dim lines() As String = File.ReadAllLines(fileName)
            Dim currentTech As Technology = Nothing

            For i As Integer = 0 To lines.Length - 1
                Dim line As String = lines(i).Trim()

                If line.StartsWith("BEGIN_TECH") Then
                    ' Parse BEGIN_TECH "Name" ID
                    ' Simple parsing instead of regex
                    If line.Contains("""") Then
                        Dim firstQuote As Integer = line.IndexOf("""")
                        Dim secondQuote As Integer = line.IndexOf("""", firstQuote + 1)

                        If firstQuote >= 0 AndAlso secondQuote >= 0 Then
                            currentTech = New Technology()
                            currentTech.Name = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1)

                            ' Get ID after the second quote
                            Dim remainingLine As String = line.Substring(secondQuote + 1).Trim()
                            If remainingLine.Length > 0 Then
                                currentTech.ID = remainingLine
                            End If
                        End If
                    End If

                ElseIf line.StartsWith("END_TECH") AndAlso currentTech IsNot Nothing Then
                    TechList(currentTech.ID) = currentTech
                    currentTech = Nothing

                ElseIf currentTech IsNot Nothing AndAlso line.Contains(" ") Then
                    Dim parts() As String = line.Split({" "c}, 2)
                    Dim propertyName As String = parts(0)
                    Dim propertyValue As String = parts(1).Trim()

                    Select Case propertyName.ToUpper()
                        Case "CATEGORY"
                            Dim categoryValue As Integer
                            If Integer.TryParse(propertyValue, categoryValue) Then
                                currentTech.Category = categoryValue
                            End If

                        Case "COST"
                            ' Shared cost for both factions
                            Dim costValue As Integer
                            If Integer.TryParse(propertyValue, costValue) Then
                                currentTech.EdenCost = costValue
                                currentTech.PlymouthCost = costValue
                            End If

                        Case "EDEN_COST"
                            Dim edenCostValue As Integer
                            If Integer.TryParse(propertyValue, edenCostValue) Then
                                currentTech.EdenCost = edenCostValue
                            End If

                        Case "PLYMOUTH_COST"
                            Dim plymouthCostValue As Integer
                            If Integer.TryParse(propertyValue, plymouthCostValue) Then
                                currentTech.PlymouthCost = plymouthCostValue
                            End If

                        Case "MAX_SCIENTISTS"
                            Dim maxScientistsValue As Integer
                            If Integer.TryParse(propertyValue, maxScientistsValue) Then
                                currentTech.MaxScientists = maxScientistsValue
                            End If

                        Case "LAB"
                            Dim labValue As Integer
                            If Integer.TryParse(propertyValue, labValue) Then
                                currentTech.Lab = labValue
                            End If

                        Case "DESCRIPTION"
                            currentTech.Description = propertyValue.Trim(""""c)

                        Case "TEASER"
                            currentTech.Teaser = propertyValue.Trim(""""c)

                        Case "IMPROVE_DESC"
                            currentTech.ImproveDesc = propertyValue.Trim(""""c)

                        Case "REQUIRES"
                            currentTech.Prerequisites.Add(propertyValue)

                        Case "UNIT_PROP"
                            Dim propParts() As String = propertyValue.Split({" "c}, 3)
                            If propParts.Length >= 3 Then
                                currentTech.UnitProperties.Add(New UnitProperty() With {
                                .UnitType = propParts(0),
                                .PropertyName = propParts(1),
                                .Value = propParts(2)
                            })
                            End If
                    End Select
                End If
            Next

            FilePath = fileName
            Me.Text = $"Outpost 2 Tech Tree Editor - {Path.GetFileName(fileName)}"
            PopulateTechList()
            UpdateStatusBar()

            Debug.WriteLine("File Loaded: " & Path.GetFileName(fileName))
            MessageBox.Show($"Loaded {TechList.Count} technologies from {Path.GetFileName(fileName)}",
                      "File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error loading file: {ex.Message}", "Error",
                      MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Private Sub SaveTechFile(fileName As String)
        Try
            ' Create backup of original file first
            Dim backupFilePath As String = fileName & ".bak"

            ' Remove existing backup if it exists
            If IO.File.Exists(backupFilePath) Then
                IO.File.Delete(backupFilePath)
            End If

            ' Create the backup
            IO.File.Copy(fileName, backupFilePath)
            Debug.WriteLine("Backup created: " & backupFilePath)

        Catch ex As Exception

        End Try

        Try
            Dim sb As New StringBuilder()
            sb.AppendLine(";   Technology Tree for Outpost 2")
            sb.AppendLine(";   Generated by Outpost 2 Tech Tree Editor")
            sb.AppendLine(";")
            sb.AppendLine()

            ' Group technologies by tech level (first 2 digits of ID)
            Dim techLevels As New Dictionary(Of String, List(Of Technology))()

            For Each tech In TechList.Values
                Dim level As String = tech.ID.Substring(0, 2)
                If Not techLevels.ContainsKey(level) Then
                    techLevels(level) = New List(Of Technology)()
                End If
                techLevels(level).Add(tech)
            Next

            ' Sort tech levels and output
            For Each level In techLevels.Keys.OrderBy(Function(x) x)
                sb.AppendLine(";******************************************************************************")
                sb.AppendLine($";Tech Level {level}")
                sb.AppendLine(";******************************************************************************")
                sb.AppendLine()

                For Each tech In techLevels(level).OrderBy(Function(x) x.ID)
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

                    ' Handle costs
                    If tech.IsBothFactions AndAlso tech.EdenCost = tech.PlymouthCost Then
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

                    sb.AppendLine("END_TECH")
                    sb.AppendLine()
                Next
            Next
            sb.AppendLine(";******************************************************************************")
            sb.AppendLine("; End")

            File.WriteAllText(fileName, sb.ToString())

            Debug.WriteLine("Tech file saved: " & Path.GetFileName(fileName))

            MessageBox.Show($"Tech tree saved to {Path.GetFileName(fileName)}", "File Saved",
                          MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function PromptSaveChanges() As Boolean
        ' TODO: Implement change tracking and prompt for save
        Return True
    End Function

    ' UI Population
    Private Sub PopulateTechList()
        lstTechnologies.Items.Clear()
        Dim searchText As String = txtSearch.Text.ToLower()
        Dim categoryFilter As Integer = -1

        If cmbCategoryFilter.SelectedIndex > 0 Then
            categoryFilter = cmbCategoryFilter.SelectedIndex - 1
        End If

        For Each tech In TechList.Values.OrderBy(Function(x) x.ID)
            Dim include As Boolean = True

            ' Search filter
            If Not String.IsNullOrEmpty(searchText) Then
                include = tech.Name.ToLower().Contains(searchText) OrElse
                         tech.ID.ToLower().Contains(searchText)
            End If

            ' Category filter
            If include AndAlso categoryFilter >= 0 Then
                include = tech.Category = categoryFilter
            End If

            If include Then
                lstTechnologies.Items.Add(tech.GetDisplayText())
            End If
        Next

        PopulatePrerequisiteCombo()
        UpdateStatusBar()
    End Sub

    Private Sub PopulatePrerequisiteCombo()
        cmbAvailablePrereqs.Items.Clear()
        For Each tech In TechList.Values.OrderBy(Function(x) x.ID)
            cmbAvailablePrereqs.Items.Add(tech.GetDisplayText())
        Next
    End Sub

    Private Sub UpdateStatusBar()
        lblTechCount.Text = TechList.Count.ToString()

        Dim totalCost As Integer = 0
        For Each tech In TechList.Values
            If tech.EdenCost > 0 Then totalCost += tech.EdenCost
            If tech.PlymouthCost > 0 AndAlso tech.PlymouthCost <> tech.EdenCost Then
                totalCost += tech.PlymouthCost
            End If
        Next
        lblTotalCost.Text = totalCost.ToString()
    End Sub

    ' Technology Selection and Editing
    Private Sub lstTechnologies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstTechnologies.SelectedIndexChanged
        If lstTechnologies.SelectedIndex >= 0 AndAlso Not IsLoading Then
            Dim selectedText As String = lstTechnologies.SelectedItem.ToString()
            Dim techID As String = selectedText.Split(" "c)(0)

            If TechList.ContainsKey(techID) Then
                LoadTechToForm(TechList(techID))
            End If

            ' Select and show the general tab
            TabControl1.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadTechToForm(tech As Technology)
        IsLoading = True
        CurrentTech = tech

        txtTechID.Text = tech.ID
        txtTechName.Text = tech.Name
        cmbCategory.SelectedIndex = Math.Min(tech.Category, cmbCategory.Items.Count - 1)
        txtMaxScientists.Text = If(tech.MaxScientists > 0, tech.MaxScientists.ToString(), "")
        cmbLab.SelectedIndex = tech.Lab
        txtEdenCost.Text = tech.EdenCost.ToString()
        txtPlymouthCost.Text = tech.PlymouthCost.ToString()

        ' 'EDEN_COST  0' = Free
        ' 'EDEN_COST -1' = Disabled for Eden
        chkEden.Checked = tech.IsEdenAvailable          ' true for both free (0) and paid (>0)
        chkPlymouth.Checked = tech.IsPlymouthAvailable  ' true for both free (0) and paid (>0)  
        chkBoth.Checked = tech.IsBothFactions

        txtTeaser.Text = tech.Teaser
        txtDescription.Text = tech.Description
        txtImproveDesc.Text = tech.ImproveDesc

        ' Prerequisites
        lstPrerequisites.Items.Clear()
        For Each prereq In tech.Prerequisites
            If TechList.ContainsKey(prereq) Then
                lstPrerequisites.Items.Add(TechList(prereq).GetDisplayText())
            Else
                lstPrerequisites.Items.Add(prereq)
            End If
        Next

        ' Unit Properties
        lstUnitProperties.Items.Clear()
        For Each prop In tech.UnitProperties
            Dim item As New ListViewItem(prop.UnitType)
            item.SubItems.Add(prop.PropertyName)
            item.SubItems.Add(prop.Value)
            lstUnitProperties.Items.Add(item)
        Next

        UpdateCategoryIcon()

        IsLoading = False
    End Sub

    Private Sub btnSaveTech_Click(sender As Object, e As EventArgs) Handles btnSaveTech.Click
        SaveCurrentTech()
    End Sub

    Private Sub SaveCurrentTech()
        If CurrentTech Is Nothing Then Return

        Try
            CurrentTech.ID = txtTechID.Text.Trim()
            CurrentTech.Name = txtTechName.Text.Trim()
            CurrentTech.Category = cmbCategory.SelectedIndex

            Integer.TryParse(txtMaxScientists.Text, CurrentTech.MaxScientists)
            CurrentTech.Lab = cmbLab.SelectedIndex

            Integer.TryParse(txtEdenCost.Text, CurrentTech.EdenCost)
            Integer.TryParse(txtPlymouthCost.Text, CurrentTech.PlymouthCost)

            CurrentTech.Teaser = txtTeaser.Text.Trim()
            CurrentTech.Description = txtDescription.Text.Trim()
            CurrentTech.ImproveDesc = txtImproveDesc.Text.Trim()

            PopulateTechList()
            UpdateStatusBar()

        Catch ex As Exception
            MessageBox.Show($"Error saving technology: {ex.Message}", "Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' New Technology
    Private Sub btnNewTech_Click(sender As Object, e As EventArgs) Handles btnNewTech.Click
        AddTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub AddTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddTechnologyToolStripMenuItem.Click
        Dim newTech As New Technology()

        ' Generate new ID
        Dim maxID As Integer = 0
        For Each tech In TechList.Values
            Dim id As Integer
            If Integer.TryParse(tech.ID, id) Then
                maxID = Math.Max(maxID, id)
            End If
        Next

        newTech.ID = (maxID + 1).ToString().PadLeft(5, "0"c)
        newTech.Name = "New Technology"
        newTech.EdenCost = 1000
        newTech.PlymouthCost = 1000

        TechList(newTech.ID) = newTech
        PopulateTechList()

        ' Select the new technology
        For i As Integer = 0 To lstTechnologies.Items.Count - 1
            If lstTechnologies.Items(i).ToString().StartsWith(newTech.ID) Then
                lstTechnologies.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    ' Duplicate Technology
    Private Sub btnDuplicateTech_Click(sender As Object, e As EventArgs) Handles btnDuplicateTech.Click
        DuplicateTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub DuplicateTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DuplicateTechnologyToolStripMenuItem.Click
        If CurrentTech Is Nothing Then Return

        Dim newTech As New Technology()

        ' Copy all properties
        newTech.Name = CurrentTech.Name & " (Copy)"
        newTech.Category = CurrentTech.Category
        newTech.EdenCost = CurrentTech.EdenCost
        newTech.PlymouthCost = CurrentTech.PlymouthCost
        newTech.MaxScientists = CurrentTech.MaxScientists
        newTech.Lab = CurrentTech.Lab
        newTech.Description = CurrentTech.Description
        newTech.Teaser = CurrentTech.Teaser
        newTech.ImproveDesc = CurrentTech.ImproveDesc
        newTech.TechIcon = CurrentTech.TechIcon

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

        ' Generate new ID
        Dim maxID As Integer = 0
        For Each tech In TechList.Values
            Dim id As Integer
            If Integer.TryParse(tech.ID, id) Then
                maxID = Math.Max(maxID, id)
            End If
        Next

        newTech.ID = (maxID + 1).ToString().PadLeft(5, "0"c)

        TechList(newTech.ID) = newTech
        PopulateTechList()

        ' Select the new technology
        For i As Integer = 0 To lstTechnologies.Items.Count - 1
            If lstTechnologies.Items(i).ToString().StartsWith(newTech.ID) Then
                lstTechnologies.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    ' Delete Technology
    Private Sub btnDeleteTech_Click(sender As Object, e As EventArgs) Handles btnDeleteTech.Click
        DeleteTechnologyToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub DeleteTechnologyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteTechnologyToolStripMenuItem.Click
        If CurrentTech Is Nothing Then Return

        If MessageBox.Show($"Are you sure you want to delete '{CurrentTech.Name}'? This cannot be undone.",
                          "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            TechList.Remove(CurrentTech.ID)
            PopulateTechList()
            ClearForm()
        End If
    End Sub

    ' Prerequisites Management
    Private Sub btnAddPrereq_Click(sender As Object, e As EventArgs) Handles btnAddPrereq.Click
        If cmbAvailablePrereqs.SelectedIndex >= 0 AndAlso CurrentTech IsNot Nothing Then
            Dim selectedText As String = cmbAvailablePrereqs.SelectedItem.ToString()
            Dim prereqID As String = selectedText.Split(" "c)(0)

            If Not CurrentTech.Prerequisites.Contains(prereqID) Then
                CurrentTech.Prerequisites.Add(prereqID)
                LoadTechToForm(CurrentTech) ' Refresh the form
            End If
        End If
    End Sub

    Private Sub btnRemovePrereq_Click(sender As Object, e As EventArgs) Handles btnRemovePrereq.Click
        If lstPrerequisites.SelectedIndex >= 0 AndAlso CurrentTech IsNot Nothing Then
            Dim selectedText As String = lstPrerequisites.SelectedItem.ToString()
            Dim prereqID As String = selectedText.Split(" "c)(0)

            CurrentTech.Prerequisites.Remove(prereqID)
            LoadTechToForm(CurrentTech) ' Refresh the form
        End If
    End Sub

    ' Unit Properties Management
    Private Sub btnAddUnitProp_Click(sender As Object, e As EventArgs) Handles btnAddUnitProp.Click
        If CurrentTech Is Nothing OrElse cmbUnitType.SelectedIndex < 0 OrElse
       cmbUnitProperty.SelectedIndex < 0 OrElse String.IsNullOrEmpty(txtUnitValue.Text) Then
            MessageBox.Show("Please fill in all unit property fields.", "Missing Information",
                      MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        CurrentTech.UnitProperties.Add(New UnitProperty() With {
        .UnitType = cmbUnitType.SelectedItem.ToString(),
        .PropertyName = cmbUnitProperty.SelectedItem.ToString(),
        .Value = txtUnitValue.Text.Trim()
        })

        cmbUnitType.SelectedIndex = -1
        cmbUnitProperty.SelectedIndex = -1
        txtUnitValue.Text = ""

        LoadTechToForm(CurrentTech) ' Refresh the form
    End Sub

    Private Sub btnRemoveUnitProp_Click(sender As Object, e As EventArgs) Handles btnRemoveUnitProp.Click
        If lstUnitProperties.SelectedIndices.Count > 0 AndAlso CurrentTech IsNot Nothing Then
            Dim index As Integer = lstUnitProperties.SelectedIndices(0)
            CurrentTech.UnitProperties.RemoveAt(index)
            LoadTechToForm(CurrentTech) ' Refresh the form
        End If
    End Sub

    ' Search and Filtering
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        PopulateTechList()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        PopulateTechList()
    End Sub

    Private Sub cmbCategoryFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategoryFilter.SelectedIndexChanged
        PopulateTechList()
    End Sub

    ' Cost checkboxes
    Private Sub chkEden_CheckedChanged(sender As Object, e As EventArgs) Handles chkEden.CheckedChanged
        If Not IsLoading Then
            If chkEden.Checked Then
                If txtEdenCost.Text = "-1" Then
                    txtEdenCost.Text = "1000"  ' Default to normal cost
                End If
            Else
                txtEdenCost.Text = "-1"  ' Disable for Eden
            End If
            UpdateBothCheckbox()
        End If
    End Sub

    Private Sub chkPlymouth_CheckedChanged(sender As Object, e As EventArgs) Handles chkPlymouth.CheckedChanged
        If Not IsLoading Then
            If chkPlymouth.Checked Then
                If txtPlymouthCost.Text = "-1" Then
                    txtPlymouthCost.Text = "1000"  ' Default to normal cost
                End If
            Else
                txtPlymouthCost.Text = "-1"  ' Disable for Plymouth
            End If
            UpdateBothCheckbox()
        End If
    End Sub

    Private Sub chkBoth_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoth.CheckedChanged
        If Not IsLoading Then
            If chkBoth.Checked Then
                chkEden.Checked = True
                chkPlymouth.Checked = True
                If txtEdenCost.Text = "-1" Then txtEdenCost.Text = "1000"
                If txtPlymouthCost.Text = "-1" Then txtPlymouthCost.Text = "1000"
            End If
        End If
    End Sub

    Private Sub UpdateBothCheckbox()
        If Not IsLoading Then
            chkBoth.Checked = chkEden.Checked AndAlso chkPlymouth.Checked
        End If
    End Sub

    ' Help and About
    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        MessageBox.Show("Outpost 2 Tech Tree Editor" & vbCrLf & vbCrLf &
                       "- Load existing tech files using File > Open" & vbCrLf &
                       "- Create new technologies with 'Create New Technology'" & vbCrLf &
                       "- Duplicate existing technologies as templates" & vbCrLf &
                       "- Use -1 cost to make tech unavailable to a faction" & vbCrLf &
                       "- Add prerequisites to control research order" & vbCrLf &
                       "- Use Unit Properties tab to modify game units" & vbCrLf & vbCrLf &
                       "Tech ID format: XXYYZ (XX=level, YY=category, Z=variant)",
                       "Help", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MessageBox.Show("Outpost 2 Tech Tree Editor" & vbCrLf & vbCrLf &
                       "Create and modify technology trees for Outpost 2: Divided Destiny" & vbCrLf & vbCrLf &
                       "Version: " & Version,
                       "About", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub fMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not PromptSaveChanges() Then
            e.Cancel = True
        End If
    End Sub

    ' Validation with detailed error reporting
    Private Sub ValidateCurrentTech()
        If CurrentTech Is Nothing Then Return

        Dim errors As List(Of String) = TechValidator.ValidateTechnology(CurrentTech)

        If errors.Count > 0 Then
            Dim errorMessage As String = "Validation Errors:" & vbCrLf & vbCrLf
            For Each errorMsg In errors
                errorMessage += "• " + errorMsg + vbCrLf
            Next

            MessageBox.Show(errorMessage, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show("Technology validation passed!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' Validate entire tech tree
    Private Sub ValidateTechTree()
        Dim errors As List(Of String) = TechValidator.ValidateTechTree(TechList)

        If errors.Count > 0 Then
            Dim errorMessage As String = "Tech Tree Validation Errors:" & vbCrLf & vbCrLf
            For Each errorMsg In errors
                errorMessage += "• " + errorMsg + vbCrLf
            Next

            MessageBox.Show(errorMessage, "Tech Tree Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show("Tech tree validation passed!", "Tech Tree Validation", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' Show detailed statistics
    Private Sub ShowStatistics()
        Dim stats As Dictionary(Of String, Object) = TechImportExport.GetTechStatistics(TechList)

        Dim statsMessage As New StringBuilder()
        statsMessage.AppendLine("Tech Tree Statistics")
        statsMessage.AppendLine("===================")
        statsMessage.AppendLine()
        statsMessage.AppendLine($"Total Technologies: {stats("TotalTechnologies")}")
        statsMessage.AppendLine($"Eden Only: {stats("EdenOnlyTechs")}")
        statsMessage.AppendLine($"Plymouth Only: {stats("PlymouthOnlyTechs")}")
        statsMessage.AppendLine($"Both Factions: {stats("BothFactionTechs")}")
        statsMessage.AppendLine($"Free Technologies: {stats("FreeTechs")}")
        statsMessage.AppendLine($"Researchable Technologies: {stats("ResearchableTechs")}")
        statsMessage.AppendLine()
        statsMessage.AppendLine($"Total Eden Research Cost: {stats("TotalEdenCost"):N0} RP")
        statsMessage.AppendLine($"Total Plymouth Research Cost: {stats("TotalPlymouthCost"):N0} RP")
        statsMessage.AppendLine()
        statsMessage.AppendLine("Technologies by Category:")

        Dim categoryBreakdown As Dictionary(Of Integer, Integer) = DirectCast(stats("CategoryBreakdown"), Dictionary(Of Integer, Integer))
        For Each kvp In categoryBreakdown.OrderBy(Function(x) x.Key)
            statsMessage.AppendLine($"  {kvp.Key.GetCategoryName()}: {kvp.Value}")
        Next

        MessageBox.Show(statsMessage.ToString(), "Tech Tree Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Enhanced prerequisite management with better UI
    Private Sub RefreshPrerequisiteCombo()
        cmbAvailablePrereqs.Items.Clear()

        ' Only show technologies that aren't already prerequisites and aren't the current tech
        For Each tech In TechList.Values.OrderBy(Function(x) x.ID)
            If CurrentTech IsNot Nothing AndAlso tech.ID <> CurrentTech.ID AndAlso
               Not CurrentTech.Prerequisites.Contains(tech.ID) Then
                cmbAvailablePrereqs.Items.Add(tech.GetDisplayText())
            End If
        Next
    End Sub

    ' Auto-suggest next Tech ID based on existing pattern
    Private Function SuggestNextTechId() As String
        If TechList.Count = 0 Then Return "02001"

        ' Find the highest ID in each tech level
        Dim techLevels As New Dictionary(Of String, Integer)()

        For Each tech In TechList.Values
            If tech.ID.Length >= 2 Then
                Dim level As String = tech.ID.Substring(0, 2)
                Dim idNum As Integer
                If Integer.TryParse(tech.ID, idNum) Then
                    If Not techLevels.ContainsKey(level) OrElse techLevels(level) < idNum Then
                        techLevels(level) = idNum
                    End If
                End If
            End If
        Next

        ' Suggest next ID in the highest tech level
        If techLevels.Count > 0 Then
            Dim maxId As Integer = techLevels.Values.Max()
            Return (maxId + 1).ToString().PadLeft(5, "0"c)
        End If

        Return "02001"
    End Function

    ' Export to CSV functionality
    Private Sub ExportToCSV()
        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
        saveDialog.Title = "Export Tech Tree to CSV"
        saveDialog.FileName = "techTree.csv"

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Try
                TechImportExport.ExportToCSV(TechList, saveDialog.FileName)
                MessageBox.Show($"Tech tree exported to {Path.GetFileName(saveDialog.FileName)}",
                              "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Export Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' Create sample tech tree for testing
    Private Sub CreateSampleTechTree()
        If MessageBox.Show("This will replace the current tech tree with a sample. Continue?",
                          "Create Sample", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            TechList = TechFileGenerator.CreateBasicTechTree()
            FilePath = ""
            Me.Text = "Outpost 2 Tech Tree Editor - Sample Tech Tree"
            PopulateTechList()
            UpdateStatusBar()
            MessageBox.Show("Sample tech tree created!", "Sample Created", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' Smart cost suggestions
    Private Sub SuggestTechCosts()
        If CurrentTech Is Nothing Then Return

        ' Suggest costs based on category and tech level
        Dim techLevel As Integer = CurrentTech.GetTechLevel()
        Dim baseCost As Integer = 500 + (techLevel * 200)

        Select Case CurrentTech.Category
            Case 0 ' Basic/Structures - usually free or low cost
                baseCost = Math.Max(0, baseCost - 300)
            Case 1 ' Database Recovery - low cost
                baseCost = Math.Max(200, baseCost - 200)
            Case 7, 8, 12 ' Weapons, Space, Starship - high cost
                baseCost += 500
            Case 4, 6 ' Vehicles, Mining - medium-high cost
                baseCost += 200
        End Select

        If txtEdenCost.Text = "" OrElse txtEdenCost.Text = "0" Then
            txtEdenCost.Text = baseCost.ToString()
        End If

        If txtPlymouthCost.Text = "" OrElse txtPlymouthCost.Text = "0" Then
            txtPlymouthCost.Text = baseCost.ToString()
        End If
    End Sub

    ' Find technologies that reference the current one
    Private Function FindReferencingTechs(techId As String) As List(Of String)
        Dim references As New List(Of String)

        For Each tech In TechList.Values
            If tech.Prerequisites.Contains(techId) Then
                references.Add(tech.GetDisplayText())
            End If
        Next

        Return references
    End Function

    ' Show dependency tree
    Private Sub ShowDependencyTree()
        If CurrentTech Is Nothing Then Return

        Dim dependencies As New StringBuilder()
        dependencies.AppendLine($"Dependency Tree for: {CurrentTech.Name}")
        dependencies.AppendLine(New String("="c, 39))
        dependencies.AppendLine()

        ' Prerequisites (what this tech needs)
        dependencies.AppendLine("Prerequisites:")
        If CurrentTech.Prerequisites.Count = 0 Then
            dependencies.AppendLine("  None")
        Else
            For Each prereqId In CurrentTech.Prerequisites
                If TechList.ContainsKey(prereqId) Then
                    dependencies.AppendLine($"  • {TechList(prereqId).GetDisplayText()}")
                Else
                    dependencies.AppendLine($"  • {prereqId} (Missing!)")
                End If
            Next
        End If

        dependencies.AppendLine()

        ' Dependents (what needs this tech)
        Dim dependents As List(Of String) = FindReferencingTechs(CurrentTech.ID)
        dependencies.AppendLine("Required by:")
        If dependents.Count = 0 Then
            dependencies.AppendLine("  None")
        Else
            For Each dependent In dependents
                dependencies.AppendLine($"  • {dependent}")
            Next
        End If

        MessageBox.Show(dependencies.ToString(), "Dependency Tree", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

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

    Private Sub CreateSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateSampleToolStripMenuItem.Click
        CreateSampleTechTree()
    End Sub

    Private Sub ShowDependenciesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowDependenciesToolStripMenuItem.Click
        ShowDependencyTree()
    End Sub

    ' Enhanced form initialization
    Private Sub InitializeAdvancedFeatures()
        ' Set up auto-complete for Tech ID
        txtTechID.AutoCompleteMode = AutoCompleteMode.Suggest
        txtTechID.AutoCompleteSource = AutoCompleteSource.CustomSource

        ' Set up tooltips with more detailed information
        ToolTip1.SetToolTip(txtTechID, "Enter a 5-digit technology ID. Format: XXYYZ where XX=tech level, YY=category, Z=variant")
        ToolTip1.SetToolTip(cmbCategory, "Select the technology category. This affects research tree organization and AI behavior.")
        ToolTip1.SetToolTip(txtMaxScientists, "Maximum scientists that can work on this technology (1-20). Higher numbers allow faster research.")
        ToolTip1.SetToolTip(cmbLab, "Laboratory level required. Higher levels are unlocked through research.")

        ' Initialize auto-complete sources
        PopulateAutoCompleteData()
    End Sub

    Private Sub PopulateAutoCompleteData()
        ' Populate tech ID suggestions
        txtTechID.AutoCompleteCustomSource.Clear()
        For i As Integer = 1 To 99
            For j As Integer = 0 To 12
                txtTechID.AutoCompleteCustomSource.Add($"{i:D2}{j:D2}0")
                txtTechID.AutoCompleteCustomSource.Add($"{i:D2}{j:D2}1")
                txtTechID.AutoCompleteCustomSource.Add($"{i:D2}{j:D2}2")
            Next
        Next
    End Sub

    Private Sub cmbUnitType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUnitType.SelectedIndexChanged
        '
    End Sub

    Private Sub txtTechID_TextChanged(sender As Object, e As EventArgs) Handles txtTechID.TextChanged
        If Not IsLoading AndAlso txtTechID.Text.IsValidTechId() Then
            SuggestTechCosts()
        End If
    End Sub

    Private Sub cmbCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCategory.SelectedIndexChanged
        If Not IsLoading Then
            SuggestTechCosts()
            UpdateCategoryIcon()
        End If
    End Sub

    Private Sub UpdateCategoryIcon()
        Try
            Dim categoryIndex As Integer = cmbCategory.SelectedIndex

            ' Clear any existing image
            pnlResearchIcon.BackgroundImage = Nothing

            ' Set image based on category (categories 1-12 map to icon1-icon12)
            Select Case categoryIndex
                Case 0  ' Blank/No Icon/Free
                    pnlResearchIcon.BackgroundImage = Nothing
                Case 1  ' Lab Icon / Database Recovery
                    pnlResearchIcon.BackgroundImage = My.Resources.icon1
                Case 2  ' Factory Icon / Infrastructure
                    pnlResearchIcon.BackgroundImage = My.Resources.icon2
                Case 3  ' Power Icon / Power Generation
                    pnlResearchIcon.BackgroundImage = My.Resources.icon3
                Case 4  ' Vehicle Icon / Vehicles & Robotics
                    pnlResearchIcon.BackgroundImage = My.Resources.icon4
                Case 5  ' Food Icon / Food Production
                    pnlResearchIcon.BackgroundImage = My.Resources.icon5
                Case 6  ' Mining Icon / Mining & Metals
                    pnlResearchIcon.BackgroundImage = My.Resources.icon6
                Case 7  ' Weapons Icon / Weapons
                    pnlResearchIcon.BackgroundImage = My.Resources.icon7
                Case 8  ' Spaceport Icon / Space Program
                    pnlResearchIcon.BackgroundImage = My.Resources.icon8
                Case 9  ' Morale Icon / Population
                    pnlResearchIcon.BackgroundImage = My.Resources.icon9
                Case 10 ' Geological Icon / Disasters
                    pnlResearchIcon.BackgroundImage = My.Resources.icon10
                Case 11 ' Emergency Icon / Medical & Emergency
                    pnlResearchIcon.BackgroundImage = My.Resources.icon11
                Case 12 ' Starship Icon / Starship Components
                    pnlResearchIcon.BackgroundImage = My.Resources.icon12
            End Select

        Catch ex As Exception
            ' Handle any resource loading errors silently
            pnlResearchIcon.BackgroundImage = Nothing
        End Try
    End Sub

    Private Sub lstPrerequisites_DoubleClick(sender As Object, e As EventArgs) Handles lstPrerequisites.DoubleClick
        If lstPrerequisites.SelectedIndex >= 0 Then
            Dim selectedText As String = lstPrerequisites.SelectedItem.ToString()
            Dim techID As String = selectedText.Split(" "c)(0)
            Clipboard.SetText(techID)

            ' Optional: Show a brief confirmation
            ToolTip1.Show($"Copied: {techID}", lstPrerequisites, 0, -20, 1000)
        End If
    End Sub

End Class
