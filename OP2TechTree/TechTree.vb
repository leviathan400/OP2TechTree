' Constants and utility classes for Outpost 2 Tech Tree Editor

Imports System.IO
Imports System.Text

Public Class TechConstants
    ' Technology Categories
    Public Shared ReadOnly Categories As New Dictionary(Of Integer, String) From {
        {0, "No Icon / Blank / Free"},
        {1, "Database Recovery"},
        {2, "Infrastructure"},
        {3, "Power Generation"},
        {4, "Vehicles & Robotics"},
        {5, "Food Production"},
        {6, "Mining & Metals"},
        {7, "Weapons"},
        {8, "Space Program"},
        {9, "Morale & Population"},
        {10, "Disasters"},
        {11, "Medical & Emergency"},
        {12, "Starship Components"}
    }

    ' Laboratory Levels
    Public Shared ReadOnly LabLevels As New Dictionary(Of Integer, String) From {
        {0, "None"},
        {1, "Basic Lab"},
        {2, "Standard Lab"},
        {3, "Advanced Lab"}
    }

    ' Canonical Unit Type list (matches README and form combo box - single source of truth).
    Public Shared ReadOnly UnitTypes() As String = {
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

    ' Canonical Unit Property list (sorted alphabetically for the form combo).
    Public Shared ReadOnly UnitProperties() As String = {
        "Armor", "Common_Required", "Concussion_Damage", "Hit_Points",
        "Improved", "Move_Speed", "Penetration_Damage", "Power_Required",
        "Production_Capacity", "Production_Rate", "Rate_Of_Fire", "Rare_Required",
        "Sight_Range", "Storage_Capacity", "Workers_Required"
    }

    ' Valid Tech ID Pattern
    Public Shared ReadOnly TechIdPattern As String = "^\d{5}$"

    ' Cost constants
    Public Const NOT_AVAILABLE As Integer = -1
    Public Const FREE_TECH As Integer = 0
    Public Const DEFAULT_COST As Integer = 1000

    ' Scientist limits
    Public Const MIN_SCIENTISTS As Integer = 1
    Public Const MAX_SCIENTISTS As Integer = 18

    ' Per-tech limits
    Public Const MAX_PREREQUISITES As Integer = 32
    Public Const MAX_UNIT_PROPERTIES As Integer = 32
    Public Const MAX_TECHS As Integer = 256
    Public Const MAX_NAME_LENGTH As Integer = 100
    Public Const MAX_TEXT_LENGTH As Integer = 800
End Class

Public Class TechValidator
    Public Shared Function ValidateTechnology(tech As fMain.Technology) As List(Of String)
        Dim errors As New List(Of String)

        ' Validate Tech ID
        If String.IsNullOrEmpty(tech.ID) Then
            errors.Add("Tech ID is required")
        ElseIf Not System.Text.RegularExpressions.Regex.IsMatch(tech.ID, TechConstants.TechIdPattern) Then
            errors.Add("Tech ID must be exactly 5 digits")
        Else
            Dim techIdNum As Integer
            If Integer.TryParse(tech.ID, techIdNum) Then
                If techIdNum < 0 Then
                    errors.Add("Tech ID should not be negative")
                End If

                Dim techLevel As Integer = techIdNum \ 1000
                If techLevel = 0 Then
                    errors.Add("Tech level (techId \ 1000) must not be 0")
                End If
            Else
                errors.Add("Tech ID must be a valid integer")
            End If
        End If

        ' Validate Tech Name
        If String.IsNullOrEmpty(tech.Name) Then
            errors.Add("Tech Name is required")
        ElseIf tech.Name.Length > TechConstants.MAX_NAME_LENGTH Then
            errors.Add($"Tech Name must not exceed {TechConstants.MAX_NAME_LENGTH} characters (currently {tech.Name.Length})")
        End If

        If tech.Teaser.Length > TechConstants.MAX_TEXT_LENGTH Then
            errors.Add($"Teaser must not exceed {TechConstants.MAX_TEXT_LENGTH} characters (currently {tech.Teaser.Length})")
        End If

        If tech.Description.Length > TechConstants.MAX_TEXT_LENGTH Then
            errors.Add($"Description must not exceed {TechConstants.MAX_TEXT_LENGTH} characters (currently {tech.Description.Length})")
        End If

        If tech.ImproveDesc.Length > TechConstants.MAX_TEXT_LENGTH Then
            errors.Add($"Improvement Description must not exceed {TechConstants.MAX_TEXT_LENGTH} characters (currently {tech.ImproveDesc.Length})")
        End If

        ' Validate Category
        If tech.Category < 0 OrElse tech.Category > 12 Then
            errors.Add("Category must be between 0 and 12")
        End If

        ' Validate Costs (must be -1, 0, or positive)
        If tech.EdenCost < -1 Then
            errors.Add("Eden Cost must be -1 (disabled), 0 (free), or positive")
        End If
        If tech.PlymouthCost < -1 Then
            errors.Add("Plymouth Cost must be -1 (disabled), 0 (free), or positive")
        End If

        If tech.EdenCost = -1 AndAlso tech.PlymouthCost = -1 Then
            errors.Add("Technology must be available to at least one faction")
        End If

        ' Validate Scientists (4-18 inclusive, 1 allowed for special non-lab techs, 0 means unset)
        If tech.MaxScientists > 0 Then
            If tech.MaxScientists = 1 Then
                ' Special case
            ElseIf tech.MaxScientists < 4 OrElse tech.MaxScientists > 18 Then
                errors.Add("Max Scientists must be 1 (special case) or between 4 and 18 inclusive")
            End If
        End If

        ' Validate Lab Level
        If tech.Lab < 0 OrElse tech.Lab > 3 Then
            errors.Add("Lab level must be 0 (None), 1 (Basic), 2 (Standard), or 3 (Advanced)")
        End If

        ' Validate Prerequisites count
        If tech.Prerequisites.Count > TechConstants.MAX_PREREQUISITES Then
            errors.Add($"Maximum of {TechConstants.MAX_PREREQUISITES} prerequisites allowed (currently {tech.Prerequisites.Count})")
        End If

        ' Validate Unit Properties count
        If tech.UnitProperties.Count > TechConstants.MAX_UNIT_PROPERTIES Then
            errors.Add($"Maximum of {TechConstants.MAX_UNIT_PROPERTIES} unit properties allowed (currently {tech.UnitProperties.Count})")
        End If

        ' Validate Unit Properties
        For Each prop In tech.UnitProperties
            If String.IsNullOrEmpty(prop.UnitType) Then
                errors.Add("Unit Property missing Unit Type")
            End If
            If String.IsNullOrEmpty(prop.PropertyName) Then
                errors.Add("Unit Property missing Property Name")
            End If
            If String.IsNullOrEmpty(prop.Value) Then
                errors.Add("Unit Property missing Value")
            Else
                Dim propValue As Integer
                If Not Integer.TryParse(prop.Value, propValue) Then
                    errors.Add($"Unit Property value '{prop.Value}' for {prop.UnitType}.{prop.PropertyName} must be a valid integer")
                Else
                    If prop.PropertyName.ToUpper() = "ARMOR" Then
                        If propValue < 0 OrElse propValue > 5 Then
                            errors.Add($"Armor property must be between 0 and 5 inclusive (currently {propValue})")
                        End If
                    End If
                End If
            End If
        Next

        Return errors
    End Function

    Public Shared Function ValidateTechTree(techList As Dictionary(Of String, fMain.Technology)) As List(Of String)
        Dim errors As New List(Of String)

        If techList.Count > TechConstants.MAX_TECHS Then
            errors.Add($"Maximum of {TechConstants.MAX_TECHS} technologies allowed (currently {techList.Count})")
        End If

        ' Dictionary is already keyed by ID, so duplicates can't physically exist here.
        ' Still scan to catch the case where tech.ID disagrees with its dictionary key.
        Dim seen As New HashSet(Of String)
        For Each kvp In techList
            If kvp.Key <> kvp.Value.ID Then
                errors.Add($"Tech dictionary key '{kvp.Key}' disagrees with stored ID '{kvp.Value.ID}'")
            End If
            If Not seen.Add(kvp.Value.ID) Then
                errors.Add($"Duplicate Tech ID: {kvp.Value.ID}")
            End If
        Next

        ' Check prerequisites exist and are valid tech IDs
        For Each tech In techList.Values
            For Each prereqId In tech.Prerequisites
                If String.IsNullOrEmpty(prereqId) Then
                    errors.Add($"Tech {tech.ID} has empty prerequisite")
                ElseIf Not System.Text.RegularExpressions.Regex.IsMatch(prereqId, TechConstants.TechIdPattern) Then
                    errors.Add($"Tech {tech.ID} has invalid prerequisite ID format: {prereqId}")
                ElseIf Not techList.ContainsKey(prereqId) Then
                    errors.Add($"Tech {tech.ID} requires missing prerequisite: {prereqId}")
                End If
            Next
        Next

        ' Global circular-dependency check using three-colour DFS (white = unvisited, gray = on stack, black = done).
        Const WHITE As Integer = 0
        Const GRAY As Integer = 1
        Const BLACK As Integer = 2
        Dim color As New Dictionary(Of String, Integer)
        For Each id In techList.Keys
            color(id) = WHITE
        Next

        Dim cycleReported As New HashSet(Of String)
        For Each rootId In techList.Keys
            If color(rootId) <> WHITE Then Continue For

            Dim stack As New Stack(Of Tuple(Of String, Integer))
            stack.Push(Tuple.Create(rootId, 0))
            color(rootId) = GRAY

            While stack.Count > 0
                Dim top = stack.Peek()
                Dim currentId = top.Item1
                Dim idx = top.Item2
                Dim prereqs = techList(currentId).Prerequisites

                If idx >= prereqs.Count Then
                    color(currentId) = BLACK
                    stack.Pop()
                Else
                    Dim nextId = prereqs(idx)
                    stack.Pop()
                    stack.Push(Tuple.Create(currentId, idx + 1))

                    If techList.ContainsKey(nextId) Then
                        If color(nextId) = GRAY Then
                            If cycleReported.Add(nextId) Then
                                errors.Add($"Circular dependency detected involving Tech {nextId}")
                            End If
                        ElseIf color(nextId) = WHITE Then
                            color(nextId) = GRAY
                            stack.Push(Tuple.Create(nextId, 0))
                        End If
                    End If
                End If
            End While
        Next

        Return errors
    End Function
End Class

Public Class TechFileGenerator
    Public Shared Function GenerateSampleTech() As fMain.Technology
        Return New fMain.Technology() With {
            .ID = "05999",
            .Name = "Sample Technology",
            .Category = 4,
            .EdenCost = 1500,
            .PlymouthCost = 1200,
            .MaxScientists = 12,
            .Lab = 2,
            .Teaser = "This is a sample technology for demonstration purposes. It shows how to structure technology definitions in the Outpost 2 tech tree format.",
            .Description = "Sample Technology research has been completed. This technology demonstrates the proper format and structure for creating new research items in Outpost 2 tech trees.",
            .ImproveDesc = "Improved sample capability",
            .Prerequisites = New List(Of String) From {"03000", "03401"},
            .UnitProperties = New List(Of fMain.UnitProperty) From {
                New fMain.UnitProperty() With {.UnitType = "TOKAMAK", .PropertyName = "Production_Capacity", .Value = "350"},
                New fMain.UnitProperty() With {.UnitType = "SCOUT", .PropertyName = "Move_Speed", .Value = "3"}
            }
        }
    End Function

    Public Shared Function CreateBasicTechTree() As Dictionary(Of String, fMain.Technology)
        Dim techList As New Dictionary(Of String, fMain.Technology)

        For level As Integer = 1 To 5
            Dim techLevelTech As New fMain.Technology() With {
                .ID = $"{level:D2}000",
                .Name = $"Tech Level {level}",
                .Category = 0,
                .EdenCost = 0,
                .PlymouthCost = 0
            }
            techList(techLevelTech.ID) = techLevelTech
        Next

        techList("05999") = GenerateSampleTech()

        Return techList
    End Function
End Class

Public Class TechImportExport
    Private Shared Function CsvEscape(s As String) As String
        ' Escape quotes and replace embedded CR/LF with spaces so the cell stays on one CSV line.
        Dim escaped As String = s.Replace("""", """""")
        escaped = escaped.Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
        Return escaped
    End Function

    Public Shared Sub ExportToCSV(techList As Dictionary(Of String, fMain.Technology), fileName As String)
        Try
            Using writer As New StreamWriter(fileName)
                writer.WriteLine("ID,Name,Category,EdenCost,PlymouthCost,MaxScientists,Lab,Teaser,Description,ImproveDesc,Prerequisites,UnitProperties")

                Dim sortedTechs = techList.Values.OrderBy(Function(x) x.ID).ToList()

                For Each tech In sortedTechs
                    Dim prereqString As String = String.Join(";", tech.Prerequisites)

                    Dim unitPropParts As New List(Of String)
                    For Each prop In tech.UnitProperties
                        unitPropParts.Add($"{prop.UnitType}:{prop.PropertyName}:{prop.Value}")
                    Next
                    Dim unitPropString As String = String.Join(";", unitPropParts)

                    Dim line As String = String.Format("""{0}"",""{1}"",{2},{3},{4},{5},{6},""{7}"",""{8}"",""{9}"",""{10}"",""{11}""",
                        tech.ID,
                        CsvEscape(tech.Name),
                        tech.Category,
                        tech.EdenCost,
                        tech.PlymouthCost,
                        tech.MaxScientists,
                        tech.Lab,
                        CsvEscape(tech.Teaser),
                        CsvEscape(tech.Description),
                        CsvEscape(tech.ImproveDesc),
                        prereqString,
                        unitPropString)

                    writer.WriteLine(line)
                Next
            End Using
        Catch ex As Exception
            Throw New Exception("Error exporting to CSV: " + ex.Message)
        End Try
    End Sub

    Public Shared Function GetTechStatistics(techList As Dictionary(Of String, fMain.Technology)) As Dictionary(Of String, Object)
        Dim stats As New Dictionary(Of String, Object)

        Dim totalTechs As Integer = techList.Count
        Dim edenOnlyCount As Integer = 0
        Dim plymouthOnlyCount As Integer = 0
        Dim bothFactionsCount As Integer = 0
        Dim freeTechsCount As Integer = 0
        Dim researchableTechsCount As Integer = 0

        Dim totalEdenCost As Integer = 0
        Dim totalPlymouthCost As Integer = 0

        Dim categoryBreakdown As New Dictionary(Of Integer, Integer)

        For Each tech In techList.Values
            Dim isEdenAvailable As Boolean = (tech.EdenCost >= 0)
            Dim isPlymouthAvailable As Boolean = (tech.PlymouthCost >= 0)

            If isEdenAvailable AndAlso isPlymouthAvailable Then
                bothFactionsCount += 1
            ElseIf isEdenAvailable Then
                edenOnlyCount += 1
            ElseIf isPlymouthAvailable Then
                plymouthOnlyCount += 1
            End If

            If tech.EdenCost = 0 OrElse tech.PlymouthCost = 0 Then
                freeTechsCount += 1
            End If

            If tech.EdenCost > 0 OrElse tech.PlymouthCost > 0 Then
                researchableTechsCount += 1
            End If

            If tech.EdenCost > 0 Then totalEdenCost += tech.EdenCost
            If tech.PlymouthCost > 0 Then totalPlymouthCost += tech.PlymouthCost

            If categoryBreakdown.ContainsKey(tech.Category) Then
                categoryBreakdown(tech.Category) += 1
            Else
                categoryBreakdown(tech.Category) = 1
            End If
        Next

        stats("TotalTechnologies") = totalTechs
        stats("EdenOnlyTechs") = edenOnlyCount
        stats("PlymouthOnlyTechs") = plymouthOnlyCount
        stats("BothFactionTechs") = bothFactionsCount
        stats("FreeTechs") = freeTechsCount
        stats("ResearchableTechs") = researchableTechsCount
        stats("TotalEdenCost") = totalEdenCost
        stats("TotalPlymouthCost") = totalPlymouthCost
        stats("CategoryBreakdown") = categoryBreakdown

        Return stats
    End Function
End Class

Public Module TechExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function GetCategoryName(categoryId As Integer) As String
        If TechConstants.Categories.ContainsKey(categoryId) Then
            Return TechConstants.Categories(categoryId)
        End If
        Return $"Unknown Category {categoryId}"
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function GetLabName(labLevel As Integer) As String
        If TechConstants.LabLevels.ContainsKey(labLevel) Then
            Return TechConstants.LabLevels(labLevel)
        End If
        Return $"Unknown Lab Level {labLevel}"
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function IsValidTechId(techId As String) As Boolean
        Return Not String.IsNullOrEmpty(techId) AndAlso
               System.Text.RegularExpressions.Regex.IsMatch(techId, TechConstants.TechIdPattern)
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function GetTechLevel(tech As fMain.Technology) As Integer
        If tech.ID.Length >= 5 Then
            Dim idNum As Integer
            If Integer.TryParse(tech.ID, idNum) Then
                Return idNum \ 1000
            End If
        End If
        Return 0
    End Function
End Module
