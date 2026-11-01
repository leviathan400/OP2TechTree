<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class fMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddTechnologyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteTechnologyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DuplicateTechnologyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateCurrentTechToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateTechTreeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatisticsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportToCSVToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewTechTreeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportToHTMLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowDependenciesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbCategoryFilter = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lstTechnologies = New System.Windows.Forms.ListBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.txtImproveDesc = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtTeaser = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lstPrerequisites = New System.Windows.Forms.ListBox()
        Me.btnRemovePrereq = New System.Windows.Forms.Button()
        Me.btnAddPrereq = New System.Windows.Forms.Button()
        Me.cmbAvailablePrereqs = New System.Windows.Forms.ComboBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.chkBoth = New System.Windows.Forms.CheckBox()
        Me.chkPlymouth = New System.Windows.Forms.CheckBox()
        Me.chkEden = New System.Windows.Forms.CheckBox()
        Me.txtPlymouthCost = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtEdenCost = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cmbLab = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtMaxScientists = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.cmbCategory = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtTechName = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtTechID = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.tlpDependencies = New System.Windows.Forms.TableLayoutPanel()
        Me.grpRequires = New System.Windows.Forms.GroupBox()
        Me.tvRequires = New System.Windows.Forms.TreeView()
        Me.grpRequiredBy = New System.Windows.Forms.GroupBox()
        Me.tvRequiredBy = New System.Windows.Forms.TreeView()
        Me.lblDependencyHint = New System.Windows.Forms.Label()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.cmbUnitProperty = New System.Windows.Forms.ComboBox()
        Me.btnRemoveUnitProp = New System.Windows.Forms.Button()
        Me.btnAddUnitProp = New System.Windows.Forms.Button()
        Me.lstUnitProperties = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.txtUnitValue = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.cmbUnitType = New System.Windows.Forms.ComboBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btnDeleteTech = New System.Windows.Forms.Button()
        Me.btnDuplicateTech = New System.Windows.Forms.Button()
        Me.btnNewTech = New System.Windows.Forms.Button()
        Me.btnSaveTech = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblTechCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblTotalCost = New System.Windows.Forms.ToolStripStatusLabel()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.pnlResearchIcon = New System.Windows.Forms.Panel()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.tlpDependencies.SuspendLayout()
        Me.grpRequires.SuspendLayout()
        Me.grpRequiredBy.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1200, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.CloseToolStripMenuItem, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.NewToolStripMenuItem.Text = "&New"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.OpenToolStripMenuItem.Text = "&Open"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SaveToolStripMenuItem.Text = "&Save"
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save &As..."
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.CloseToolStripMenuItem.Text = "&Close Tech Tree"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(177, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddTechnologyToolStripMenuItem, Me.DeleteTechnologyToolStripMenuItem, Me.DuplicateTechnologyToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "&Edit"
        '
        'AddTechnologyToolStripMenuItem
        '
        Me.AddTechnologyToolStripMenuItem.Name = "AddTechnologyToolStripMenuItem"
        Me.AddTechnologyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.T), System.Windows.Forms.Keys)
        Me.AddTechnologyToolStripMenuItem.Size = New System.Drawing.Size(230, 22)
        Me.AddTechnologyToolStripMenuItem.Text = "&Add Technology"
        '
        'DeleteTechnologyToolStripMenuItem
        '
        Me.DeleteTechnologyToolStripMenuItem.Name = "DeleteTechnologyToolStripMenuItem"
        Me.DeleteTechnologyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.DeleteTechnologyToolStripMenuItem.Size = New System.Drawing.Size(230, 22)
        Me.DeleteTechnologyToolStripMenuItem.Text = "&Delete Technology"
        '
        'DuplicateTechnologyToolStripMenuItem
        '
        Me.DuplicateTechnologyToolStripMenuItem.Name = "DuplicateTechnologyToolStripMenuItem"
        Me.DuplicateTechnologyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D), System.Windows.Forms.Keys)
        Me.DuplicateTechnologyToolStripMenuItem.Size = New System.Drawing.Size(230, 22)
        Me.DuplicateTechnologyToolStripMenuItem.Text = "D&uplicate Technology"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ValidateCurrentTechToolStripMenuItem, Me.ValidateTechTreeToolStripMenuItem, Me.StatisticsToolStripMenuItem, Me.ToolStripSeparator2, Me.ViewTechTreeToolStripMenuItem, Me.ExportToHTMLToolStripMenuItem, Me.ExportToCSVToolStripMenuItem, Me.CreateSampleToolStripMenuItem, Me.ShowDependenciesToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'ValidateCurrentTechToolStripMenuItem
        '
        Me.ValidateCurrentTechToolStripMenuItem.Name = "ValidateCurrentTechToolStripMenuItem"
        Me.ValidateCurrentTechToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ValidateCurrentTechToolStripMenuItem.Text = "&Validate Current Tech"
        '
        'ValidateTechTreeToolStripMenuItem
        '
        Me.ValidateTechTreeToolStripMenuItem.Name = "ValidateTechTreeToolStripMenuItem"
        Me.ValidateTechTreeToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ValidateTechTreeToolStripMenuItem.Text = "Validate &Tech Tree"
        '
        'StatisticsToolStripMenuItem
        '
        Me.StatisticsToolStripMenuItem.Name = "StatisticsToolStripMenuItem"
        Me.StatisticsToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.StatisticsToolStripMenuItem.Text = "&Statistics"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(182, 6)
        '
        'ExportToCSVToolStripMenuItem
        '
        Me.ExportToCSVToolStripMenuItem.Name = "ExportToCSVToolStripMenuItem"
        Me.ExportToCSVToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ExportToCSVToolStripMenuItem.Text = "&Export to CSV..."
        '
        'ViewTechTreeToolStripMenuItem
        '
        Me.ViewTechTreeToolStripMenuItem.Name = "ViewTechTreeToolStripMenuItem"
        Me.ViewTechTreeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.ViewTechTreeToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ViewTechTreeToolStripMenuItem.Text = "View Tech Tree in &Browser"
        '
        'ExportToHTMLToolStripMenuItem
        '
        Me.ExportToHTMLToolStripMenuItem.Name = "ExportToHTMLToolStripMenuItem"
        Me.ExportToHTMLToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ExportToHTMLToolStripMenuItem.Text = "Export Tech Tree to &HTML..."
        '
        'CreateSampleToolStripMenuItem
        '
        Me.CreateSampleToolStripMenuItem.Name = "CreateSampleToolStripMenuItem"
        Me.CreateSampleToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.CreateSampleToolStripMenuItem.Text = "&Create Sample"
        '
        'ShowDependenciesToolStripMenuItem
        '
        Me.ShowDependenciesToolStripMenuItem.Name = "ShowDependenciesToolStripMenuItem"
        Me.ShowDependenciesToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ShowDependenciesToolStripMenuItem.Text = "Show &Dependencies"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.AboutToolStripMenuItem.Text = "&About"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TabControl1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1200, 604)
        Me.SplitContainer1.SplitterDistance = 300
        Me.SplitContainer1.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnSearch)
        Me.GroupBox1.Controls.Add(Me.txtSearch)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.cmbCategoryFilter)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.lstTechnologies)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(300, 604)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Technologies"
        '
        'btnSearch
        '
        Me.btnSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSearch.Location = New System.Drawing.Point(271, 19)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(23, 23)
        Me.btnSearch.TabIndex = 5
        Me.btnSearch.Text = "🔍"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'txtSearch
        '
        Me.txtSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSearch.Location = New System.Drawing.Point(54, 21)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(211, 20)
        Me.txtSearch.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Search:"
        '
        'cmbCategoryFilter
        '
        Me.cmbCategoryFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbCategoryFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCategoryFilter.FormattingEnabled = True
        Me.cmbCategoryFilter.Location = New System.Drawing.Point(54, 47)
        Me.cmbCategoryFilter.Name = "cmbCategoryFilter"
        Me.cmbCategoryFilter.Size = New System.Drawing.Size(240, 21)
        Me.cmbCategoryFilter.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Category:"
        '
        'lstTechnologies
        '
        Me.lstTechnologies.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstTechnologies.FormattingEnabled = True
        Me.lstTechnologies.IntegralHeight = False
        Me.lstTechnologies.Location = New System.Drawing.Point(6, 74)
        Me.lstTechnologies.Name = "lstTechnologies"
        Me.lstTechnologies.Size = New System.Drawing.Size(288, 524)
        Me.lstTechnologies.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(896, 564)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox6)
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.GroupBox3)
        Me.TabPage1.Controls.Add(Me.GroupBox2)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(888, 538)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox6.Controls.Add(Me.txtImproveDesc)
        Me.GroupBox6.Controls.Add(Me.Label19)
        Me.GroupBox6.Location = New System.Drawing.Point(6, 479)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(876, 53)
        Me.GroupBox6.TabIndex = 4
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Improvement Description"
        '
        'txtImproveDesc
        '
        Me.txtImproveDesc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtImproveDesc.Location = New System.Drawing.Point(113, 19)
        Me.txtImproveDesc.Name = "txtImproveDesc"
        Me.txtImproveDesc.Size = New System.Drawing.Size(757, 20)
        Me.txtImproveDesc.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtImproveDesc, "Brief description of what this technology improves (e.g., ""Increased Power output" &
        """)")
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(6, 22)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(104, 13)
        Me.Label19.TabIndex = 0
        Me.Label19.Text = "Improve Description:"
        '
        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.Controls.Add(Me.txtDescription)
        Me.GroupBox5.Controls.Add(Me.Label18)
        Me.GroupBox5.Controls.Add(Me.txtTeaser)
        Me.GroupBox5.Controls.Add(Me.Label17)
        Me.GroupBox5.Location = New System.Drawing.Point(6, 285)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(876, 188)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Descriptions"
        '
        'txtDescription
        '
        Me.txtDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDescription.Location = New System.Drawing.Point(9, 112)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDescription.Size = New System.Drawing.Size(861, 70)
        Me.txtDescription.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtDescription, "Detailed description shown after the technology is researched")
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(6, 96)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(116, 13)
        Me.Label18.TabIndex = 2
        Me.Label18.Text = "Completed Description:"
        '
        'txtTeaser
        '
        Me.txtTeaser.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTeaser.Location = New System.Drawing.Point(9, 32)
        Me.txtTeaser.Multiline = True
        Me.txtTeaser.Name = "txtTeaser"
        Me.txtTeaser.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtTeaser.Size = New System.Drawing.Size(861, 61)
        Me.txtTeaser.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtTeaser, "Description shown before the technology is researched (teaser text)")
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(6, 16)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(131, 13)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "Pre-Research Description:"
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.Controls.Add(Me.lstPrerequisites)
        Me.GroupBox4.Controls.Add(Me.btnRemovePrereq)
        Me.GroupBox4.Controls.Add(Me.btnAddPrereq)
        Me.GroupBox4.Controls.Add(Me.cmbAvailablePrereqs)
        Me.GroupBox4.Controls.Add(Me.Label16)
        Me.GroupBox4.Location = New System.Drawing.Point(6, 174)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(876, 105)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Prerequisites"
        '
        'lstPrerequisites
        '
        Me.lstPrerequisites.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstPrerequisites.FormattingEnabled = True
        Me.lstPrerequisites.IntegralHeight = False
        Me.lstPrerequisites.Location = New System.Drawing.Point(9, 45)
        Me.lstPrerequisites.Name = "lstPrerequisites"
        Me.lstPrerequisites.Size = New System.Drawing.Size(780, 54)
        Me.lstPrerequisites.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.lstPrerequisites, "Technologies that must be researched before this one")
        '
        'btnRemovePrereq
        '
        Me.btnRemovePrereq.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemovePrereq.Location = New System.Drawing.Point(795, 71)
        Me.btnRemovePrereq.Name = "btnRemovePrereq"
        Me.btnRemovePrereq.Size = New System.Drawing.Size(75, 23)
        Me.btnRemovePrereq.TabIndex = 3
        Me.btnRemovePrereq.Text = "Remove"
        Me.btnRemovePrereq.UseVisualStyleBackColor = True
        '
        'btnAddPrereq
        '
        Me.btnAddPrereq.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddPrereq.Location = New System.Drawing.Point(795, 45)
        Me.btnAddPrereq.Name = "btnAddPrereq"
        Me.btnAddPrereq.Size = New System.Drawing.Size(75, 23)
        Me.btnAddPrereq.TabIndex = 2
        Me.btnAddPrereq.Text = "Add"
        Me.btnAddPrereq.UseVisualStyleBackColor = True
        '
        'cmbAvailablePrereqs
        '
        Me.cmbAvailablePrereqs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbAvailablePrereqs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAvailablePrereqs.FormattingEnabled = True
        Me.cmbAvailablePrereqs.Location = New System.Drawing.Point(113, 19)
        Me.cmbAvailablePrereqs.Name = "cmbAvailablePrereqs"
        Me.cmbAvailablePrereqs.Size = New System.Drawing.Size(676, 21)
        Me.cmbAvailablePrereqs.TabIndex = 1
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(6, 22)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(86, 13)
        Me.Label16.TabIndex = 0
        Me.Label16.Text = "Available Techs:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.chkBoth)
        Me.GroupBox3.Controls.Add(Me.chkPlymouth)
        Me.GroupBox3.Controls.Add(Me.chkEden)
        Me.GroupBox3.Controls.Add(Me.txtPlymouthCost)
        Me.GroupBox3.Controls.Add(Me.Label15)
        Me.GroupBox3.Controls.Add(Me.txtEdenCost)
        Me.GroupBox3.Controls.Add(Me.Label14)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 105)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(876, 63)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Availability && Cost"
        '
        'chkBoth
        '
        Me.chkBoth.AutoSize = True
        Me.chkBoth.Location = New System.Drawing.Point(369, 21)
        Me.chkBoth.Name = "chkBoth"
        Me.chkBoth.Size = New System.Drawing.Size(48, 17)
        Me.chkBoth.TabIndex = 5
        Me.chkBoth.Text = "Both"
        Me.chkBoth.UseVisualStyleBackColor = True
        '
        'chkPlymouth
        '
        Me.chkPlymouth.AutoSize = True
        Me.chkPlymouth.Location = New System.Drawing.Point(297, 21)
        Me.chkPlymouth.Name = "chkPlymouth"
        Me.chkPlymouth.Size = New System.Drawing.Size(69, 17)
        Me.chkPlymouth.TabIndex = 4
        Me.chkPlymouth.Text = "Plymouth"
        Me.chkPlymouth.UseVisualStyleBackColor = True
        '
        'chkEden
        '
        Me.chkEden.AutoSize = True
        Me.chkEden.Location = New System.Drawing.Point(245, 21)
        Me.chkEden.Name = "chkEden"
        Me.chkEden.Size = New System.Drawing.Size(51, 17)
        Me.chkEden.TabIndex = 3
        Me.chkEden.Text = "Eden"
        Me.chkEden.UseVisualStyleBackColor = True
        '
        'txtPlymouthCost
        '
        Me.txtPlymouthCost.Location = New System.Drawing.Point(113, 36)
        Me.txtPlymouthCost.Name = "txtPlymouthCost"
        Me.txtPlymouthCost.Size = New System.Drawing.Size(100, 20)
        Me.txtPlymouthCost.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtPlymouthCost, "Research cost for Plymouth (-1 = not available)")
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(6, 39)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(77, 13)
        Me.Label15.TabIndex = 2
        Me.Label15.Text = "Plymouth Cost:"
        '
        'txtEdenCost
        '
        Me.txtEdenCost.Location = New System.Drawing.Point(113, 16)
        Me.txtEdenCost.Name = "txtEdenCost"
        Me.txtEdenCost.Size = New System.Drawing.Size(100, 20)
        Me.txtEdenCost.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtEdenCost, "Research cost for Eden (-1 = not available)")
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(6, 19)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(59, 13)
        Me.Label14.TabIndex = 0
        Me.Label14.Text = "Eden Cost:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.pnlResearchIcon)
        Me.GroupBox2.Controls.Add(Me.cmbLab)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.txtMaxScientists)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.cmbCategory)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.txtTechName)
        Me.GroupBox2.Controls.Add(Me.Label9)
        Me.GroupBox2.Controls.Add(Me.txtTechID)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(876, 93)
        Me.GroupBox2.TabIndex = 0
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Basic Information"
        '
        'cmbLab
        '
        Me.cmbLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLab.FormattingEnabled = True
        Me.cmbLab.Location = New System.Drawing.Point(113, 39)
        Me.cmbLab.Name = "cmbLab"
        Me.cmbLab.Size = New System.Drawing.Size(130, 21)
        Me.cmbLab.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.cmbLab, "Laboratory level required to research this technology")
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(281, 42)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(82, 13)
        Me.Label13.TabIndex = 10
        Me.Label13.Text = "Category (Icon):"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'txtMaxScientists
        '
        Me.txtMaxScientists.Location = New System.Drawing.Point(113, 64)
        Me.txtMaxScientists.Name = "txtMaxScientists"
        Me.txtMaxScientists.Size = New System.Drawing.Size(100, 20)
        Me.txtMaxScientists.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtMaxScientists, "Maximum number of scientists that can work on this technology (4-18)")
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(6, 67)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(78, 13)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "Max Scientists:"
        '
        'cmbCategory
        '
        Me.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCategory.FormattingEnabled = True
        Me.cmbCategory.Location = New System.Drawing.Point(369, 39)
        Me.cmbCategory.Name = "cmbCategory"
        Me.cmbCategory.Size = New System.Drawing.Size(240, 21)
        Me.cmbCategory.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cmbCategory, "Technology category (0-12)")
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(6, 42)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(28, 13)
        Me.Label10.TabIndex = 4
        Me.Label10.Text = "Lab:"
        '
        'txtTechName
        '
        Me.txtTechName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTechName.Location = New System.Drawing.Point(369, 16)
        Me.txtTechName.Name = "txtTechName"
        Me.txtTechName.Size = New System.Drawing.Size(501, 20)
        Me.txtTechName.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtTechName, "Display name of the technology")
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(297, 19)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(66, 13)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "Tech Name:"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'txtTechID
        '
        Me.txtTechID.Location = New System.Drawing.Point(113, 16)
        Me.txtTechID.Name = "txtTechID"
        Me.txtTechID.Size = New System.Drawing.Size(100, 20)
        Me.txtTechID.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtTechID, "5-digit technology ID (e.g., 05110)")
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 19)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(49, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "Tech ID:"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.GroupBox7)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(888, 538)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Unit Properties"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.tlpDependencies)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(888, 538)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Dependencies"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'tlpDependencies
        '
        Me.tlpDependencies.ColumnCount = 2
        Me.tlpDependencies.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDependencies.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDependencies.Controls.Add(Me.grpRequires, 0, 0)
        Me.tlpDependencies.Controls.Add(Me.grpRequiredBy, 1, 0)
        Me.tlpDependencies.Controls.Add(Me.lblDependencyHint, 0, 1)
        Me.tlpDependencies.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDependencies.Location = New System.Drawing.Point(3, 3)
        Me.tlpDependencies.Name = "tlpDependencies"
        Me.tlpDependencies.RowCount = 2
        Me.tlpDependencies.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDependencies.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24.0!))
        Me.tlpDependencies.Size = New System.Drawing.Size(882, 532)
        Me.tlpDependencies.TabIndex = 0
        '
        'grpRequires
        '
        Me.grpRequires.Controls.Add(Me.tvRequires)
        Me.grpRequires.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpRequires.Location = New System.Drawing.Point(3, 3)
        Me.grpRequires.Name = "grpRequires"
        Me.grpRequires.Size = New System.Drawing.Size(435, 502)
        Me.grpRequires.TabIndex = 0
        Me.grpRequires.TabStop = False
        Me.grpRequires.Text = "Requires"
        '
        'tvRequires
        '
        Me.tvRequires.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvRequires.HideSelection = False
        Me.tvRequires.Location = New System.Drawing.Point(3, 16)
        Me.tvRequires.Name = "tvRequires"
        Me.tvRequires.Size = New System.Drawing.Size(429, 483)
        Me.tvRequires.TabIndex = 0
        '
        'grpRequiredBy
        '
        Me.grpRequiredBy.Controls.Add(Me.tvRequiredBy)
        Me.grpRequiredBy.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpRequiredBy.Location = New System.Drawing.Point(444, 3)
        Me.grpRequiredBy.Name = "grpRequiredBy"
        Me.grpRequiredBy.Size = New System.Drawing.Size(435, 502)
        Me.grpRequiredBy.TabIndex = 1
        Me.grpRequiredBy.TabStop = False
        Me.grpRequiredBy.Text = "Required by"
        '
        'tvRequiredBy
        '
        Me.tvRequiredBy.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvRequiredBy.HideSelection = False
        Me.tvRequiredBy.Location = New System.Drawing.Point(3, 16)
        Me.tvRequiredBy.Name = "tvRequiredBy"
        Me.tvRequiredBy.Size = New System.Drawing.Size(429, 483)
        Me.tvRequiredBy.TabIndex = 0
        '
        'lblDependencyHint
        '
        Me.lblDependencyHint.AutoSize = True
        Me.tlpDependencies.SetColumnSpan(Me.lblDependencyHint, 2)
        Me.lblDependencyHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblDependencyHint.Location = New System.Drawing.Point(3, 513)
        Me.lblDependencyHint.Margin = New System.Windows.Forms.Padding(3, 5, 3, 0)
        Me.lblDependencyHint.Name = "lblDependencyHint"
        Me.lblDependencyHint.Size = New System.Drawing.Size(300, 13)
        Me.lblDependencyHint.TabIndex = 2
        Me.lblDependencyHint.Text = "Expand a tech to follow the chain. Double-click a tech to open it."
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.cmbUnitProperty)
        Me.GroupBox7.Controls.Add(Me.btnRemoveUnitProp)
        Me.GroupBox7.Controls.Add(Me.btnAddUnitProp)
        Me.GroupBox7.Controls.Add(Me.lstUnitProperties)
        Me.GroupBox7.Controls.Add(Me.txtUnitValue)
        Me.GroupBox7.Controls.Add(Me.Label22)
        Me.GroupBox7.Controls.Add(Me.Label21)
        Me.GroupBox7.Controls.Add(Me.cmbUnitType)
        Me.GroupBox7.Controls.Add(Me.Label20)
        Me.GroupBox7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox7.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(882, 532)
        Me.GroupBox7.TabIndex = 0
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Unit Property Modifications"
        '
        'cmbUnitProperty
        '
        Me.cmbUnitProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbUnitProperty.FormattingEnabled = True
        Me.cmbUnitProperty.Location = New System.Drawing.Point(113, 68)
        Me.cmbUnitProperty.Name = "cmbUnitProperty"
        Me.cmbUnitProperty.Size = New System.Drawing.Size(200, 21)
        Me.cmbUnitProperty.TabIndex = 9
        '
        'btnRemoveUnitProp
        '
        Me.btnRemoveUnitProp.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveUnitProp.Location = New System.Drawing.Point(801, 65)
        Me.btnRemoveUnitProp.Name = "btnRemoveUnitProp"
        Me.btnRemoveUnitProp.Size = New System.Drawing.Size(75, 23)
        Me.btnRemoveUnitProp.TabIndex = 8
        Me.btnRemoveUnitProp.Text = "Remove"
        Me.btnRemoveUnitProp.UseVisualStyleBackColor = True
        '
        'btnAddUnitProp
        '
        Me.btnAddUnitProp.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddUnitProp.Location = New System.Drawing.Point(801, 39)
        Me.btnAddUnitProp.Name = "btnAddUnitProp"
        Me.btnAddUnitProp.Size = New System.Drawing.Size(75, 23)
        Me.btnAddUnitProp.TabIndex = 7
        Me.btnAddUnitProp.Text = "Add"
        Me.btnAddUnitProp.UseVisualStyleBackColor = True
        '
        'lstUnitProperties
        '
        Me.lstUnitProperties.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstUnitProperties.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.lstUnitProperties.FullRowSelect = True
        Me.lstUnitProperties.GridLines = True
        Me.lstUnitProperties.HideSelection = False
        Me.lstUnitProperties.Location = New System.Drawing.Point(6, 94)
        Me.lstUnitProperties.Name = "lstUnitProperties"
        Me.lstUnitProperties.Size = New System.Drawing.Size(870, 432)
        Me.lstUnitProperties.TabIndex = 6
        Me.lstUnitProperties.UseCompatibleStateImageBehavior = False
        Me.lstUnitProperties.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Unit Type"
        Me.ColumnHeader1.Width = 200
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Property"
        Me.ColumnHeader2.Width = 250
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Value"
        Me.ColumnHeader3.Width = 100
        '
        'txtUnitValue
        '
        Me.txtUnitValue.Location = New System.Drawing.Point(355, 68)
        Me.txtUnitValue.Name = "txtUnitValue"
        Me.txtUnitValue.Size = New System.Drawing.Size(100, 20)
        Me.txtUnitValue.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.txtUnitValue, "New value for the property")
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(318, 71)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(37, 13)
        Me.Label22.TabIndex = 4
        Me.Label22.Text = "Value:"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(6, 71)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(49, 13)
        Me.Label21.TabIndex = 2
        Me.Label21.Text = "Property:"
        '
        'cmbUnitType
        '
        Me.cmbUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbUnitType.FormattingEnabled = True
        Me.cmbUnitType.Location = New System.Drawing.Point(113, 42)
        Me.cmbUnitType.Name = "cmbUnitType"
        Me.cmbUnitType.Size = New System.Drawing.Size(200, 21)
        Me.cmbUnitType.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cmbUnitType, "Type of unit to modify (e.g., TOKAMAK, AGRIDOME)")
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(6, 45)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(56, 13)
        Me.Label20.TabIndex = 0
        Me.Label20.Text = "Unit Type:"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnHelp)
        Me.Panel1.Controls.Add(Me.btnDeleteTech)
        Me.Panel1.Controls.Add(Me.btnDuplicateTech)
        Me.Panel1.Controls.Add(Me.btnNewTech)
        Me.Panel1.Controls.Add(Me.btnSaveTech)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 564)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(896, 40)
        Me.Panel1.TabIndex = 0
        '
        'btnHelp
        '
        Me.btnHelp.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnHelp.Location = New System.Drawing.Point(815, 9)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(75, 23)
        Me.btnHelp.TabIndex = 4
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btnDeleteTech
        '
        Me.btnDeleteTech.Location = New System.Drawing.Point(249, 9)
        Me.btnDeleteTech.Name = "btnDeleteTech"
        Me.btnDeleteTech.Size = New System.Drawing.Size(115, 23)
        Me.btnDeleteTech.TabIndex = 3
        Me.btnDeleteTech.Text = "Delete Technology"
        Me.btnDeleteTech.UseVisualStyleBackColor = True
        '
        'btnDuplicateTech
        '
        Me.btnDuplicateTech.Location = New System.Drawing.Point(128, 9)
        Me.btnDuplicateTech.Name = "btnDuplicateTech"
        Me.btnDuplicateTech.Size = New System.Drawing.Size(115, 23)
        Me.btnDuplicateTech.TabIndex = 2
        Me.btnDuplicateTech.Text = "Duplicate Technology"
        Me.btnDuplicateTech.UseVisualStyleBackColor = True
        '
        'btnNewTech
        '
        Me.btnNewTech.Location = New System.Drawing.Point(7, 9)
        Me.btnNewTech.Name = "btnNewTech"
        Me.btnNewTech.Size = New System.Drawing.Size(115, 23)
        Me.btnNewTech.TabIndex = 1
        Me.btnNewTech.Text = "Create New Technology"
        Me.btnNewTech.UseVisualStyleBackColor = True
        '
        'btnSaveTech
        '
        Me.btnSaveTech.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSaveTech.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveTech.Location = New System.Drawing.Point(724, 9)
        Me.btnSaveTech.Name = "btnSaveTech"
        Me.btnSaveTech.Size = New System.Drawing.Size(95, 23)
        Me.btnSaveTech.TabIndex = 0
        Me.btnSaveTech.Text = "Apply Changes"
        Me.btnSaveTech.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.lblTechCount, Me.ToolStripStatusLabel2, Me.lblTotalCost})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 628)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1200, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(79, 17)
        Me.ToolStripStatusLabel1.Text = "Technologies:"
        '
        'lblTechCount
        '
        Me.lblTechCount.Name = "lblTechCount"
        Me.lblTechCount.Size = New System.Drawing.Size(13, 17)
        Me.lblTechCount.Text = "0"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(71, 17)
        Me.ToolStripStatusLabel2.Text = "   Total Cost:"
        '
        'lblTotalCost
        '
        Me.lblTotalCost.Name = "lblTotalCost"
        Me.lblTotalCost.Size = New System.Drawing.Size(13, 17)
        Me.lblTotalCost.Text = "0"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.Filter = "Tech Tree Files (*.txt)|*.txt|All Files (*.*)|*.*"
        Me.OpenFileDialog1.Title = "Open Tech Tree File"
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "Tech Tree Files (*.txt)|*.txt|All Files (*.*)|*.*"
        Me.SaveFileDialog1.Title = "Save Tech Tree File"
        '
        'pnlResearchIcon
        '
        Me.pnlResearchIcon.Location = New System.Drawing.Point(825, 39)
        Me.pnlResearchIcon.Name = "pnlResearchIcon"
        Me.pnlResearchIcon.Size = New System.Drawing.Size(45, 45)
        Me.pnlResearchIcon.TabIndex = 11
        '
        'fMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 650)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(1000, 600)
        Me.Name = "fMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Outpost 2 Tech Tree Editor"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.tlpDependencies.ResumeLayout(False)
        Me.tlpDependencies.PerformLayout()
        Me.grpRequires.ResumeLayout(False)
        Me.grpRequiredBy.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddTechnologyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteTechnologyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DuplicateTechnologyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ValidateCurrentTechToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ValidateTechTreeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatisticsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ExportToCSVToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewTechTreeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportToHTMLToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CreateSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowDependenciesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmbCategoryFilter As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents lstTechnologies As ListBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents txtImproveDesc As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents txtDescription As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents txtTeaser As TextBox
    Friend WithEvents Label17 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents lstPrerequisites As ListBox
    Friend WithEvents btnRemovePrereq As Button
    Friend WithEvents btnAddPrereq As Button
    Friend WithEvents cmbAvailablePrereqs As ComboBox
    Friend WithEvents Label16 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents chkBoth As CheckBox
    Friend WithEvents chkPlymouth As CheckBox
    Friend WithEvents chkEden As CheckBox
    Friend WithEvents txtPlymouthCost As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents txtEdenCost As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents cmbLab As ComboBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtMaxScientists As TextBox
    Friend WithEvents cmbCategory As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents txtTechName As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents txtTechID As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents tlpDependencies As TableLayoutPanel
    Friend WithEvents grpRequires As GroupBox
    Friend WithEvents tvRequires As TreeView
    Friend WithEvents grpRequiredBy As GroupBox
    Friend WithEvents tvRequiredBy As TreeView
    Friend WithEvents lblDependencyHint As Label
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents btnRemoveUnitProp As Button
    Friend WithEvents btnAddUnitProp As Button
    Friend WithEvents lstUnitProperties As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents txtUnitValue As TextBox
    Friend WithEvents Label22 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents cmbUnitType As ComboBox
    Friend WithEvents Label20 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnHelp As Button
    Friend WithEvents btnDeleteTech As Button
    Friend WithEvents btnDuplicateTech As Button
    Friend WithEvents btnNewTech As Button
    Friend WithEvents btnSaveTech As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents lblTechCount As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents lblTotalCost As ToolStripStatusLabel
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents cmbUnitProperty As ComboBox
    Friend WithEvents pnlResearchIcon As Panel
End Class