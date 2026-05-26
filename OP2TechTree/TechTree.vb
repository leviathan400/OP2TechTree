' Constants and utility classes for Outpost 2 Tech Tree Editor

Imports System.IO

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
        {10, "Disaster Prediction"},
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

    ' Common Unit Types
    Public Shared ReadOnly UnitTypes() As String = {
        "AGRIDOME", "TOKAMAK", "RESIDENCE", "COMMAND_CENTER", "STRUCTURE_FACTORY",
        "SMELTER", "SMELTER_ADV", "MINE", "MINE_ADV", "MAGMA_WELL", "GORF",
        "MED_CENTER", "NURSERY", "UNIVERSITY", "RECREATION", "FORUM",
        "LIGHT_TOWER", "TOWER_GUARD", "DIRT", "OBSERVATORY", "METEOR_DEFENSE",
        "SCOUT", "LIGHT_TANK", "MED_TANK", "BIG_TANK", "CON_TRUCK", "BIG_TRUCK",
        "WALL_TRUCK", "BULL_DOZER", "MINER", "SURVEYOR", "MOBILE_HOME",
        "REPAIR_TRUCK", "SPIDER", "SCORPION", "GARAGE", "SPACEPORT",
        "GEOTHERMAL", "SOLAR_ARRAY", "FACT_STRUCTURE", "FACT_VEHICLE",
        "FACT_LUXURY", "FACT_ANDROID", "MICROWAVE", "LASER", "RAIL_GUN",
        "EMP", "FOAM", "ACID", "SPAM", "STARFLARE", "SUPERNOVA", "THORS_HAMMER"
    }

    ' Common Unit Properties
    Public Shared ReadOnly UnitProperties() As String = {
        "Production_Capacity", "Production_Rate", "Storage_Capacity",
        "Power_Required", "Workers_Required", "Hit_Points", "Armor",
        "Move_Speed", "Sight_Range", "Rate_Of_Fire", "Concussion_Damage",
        "Penetration_Damage", "Common_Required", "Rare_Required", "Improved"
    }

    ' Valid Tech ID Pattern
    Public Shared ReadOnly TechIdPattern As String = "^\d{5}$"

    ' Cost constants
    Public Const NOT_AVAILABLE As Integer = -1
    Public Const FREE_TECH As Integer = 0
    Public Const DEFAULT_COST As Integer = 1000

    ' Scientist limits
    Public Const MIN_SCIENTISTS As Integer = 1
    Public Const MAX_SCIENTISTS As Integer = 20
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
            ' Validate tech ID is not negative
            Dim techIdNum As Integer
            If Integer.TryParse(tech.ID, techIdNum) Then
                If techIdNum < 0 Then
                    errors.Add("Tech ID should not be negative")
                End If

                ' Validate tech level (techNum / 1000) is not 0
                Dim techLevel As Integer = techIdNum \ 1000
                If techLevel = 0 Then
                    errors.Add("Tech level (first 2 digits / 10) must not be 0")
                End If
            Else
                errors.Add("Tech ID must be a valid integer")
            End If
        End If

        ' Validate Tech Name
        If String.IsNullOrEmpty(tech.Name) Then
            errors.Add("Tech Name is required")
        ElseIf tech.Name.Length > 100 Then
            errors.Add("Tech Name must not exceed 100 characters (currently " & tech.Name.Length & ")")
        End If

        ' Validate string lengths (800 character limit)
        If tech.Teaser.Length > 800 Then
            errors.Add("Teaser must not exceed 800 characters (currently " & tech.Teaser.Length & ")")
        End If

        If tech.Description.Length > 800 Then
            errors.Add("Description must not exceed 800 characters (currently " & tech.Description.Length & ")")
        End If

        If tech.ImproveDesc.Length > 800 Then
            errors.Add("Improvement Description must not exceed 800 characters (currently " & tech.ImproveDesc.Length & ")")
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

        ' At least one faction must be able to research
        If tech.EdenCost = -1 AndAlso tech.PlymouthCost = -1 Then
            errors.Add("Technology must be available to at least one faction")
        End If

        ' Validate Scientists (4-18 inclusive, but 1 is allowed for special cases)
        If tech.MaxScientists > 0 Then
            If tech.MaxScientists = 1 Then
                ' Special case - allowed for built-in techs that avoid lab research
            ElseIf tech.MaxScientists < 4 OrElse tech.MaxScientists > 18 Then
                errors.Add("Max Scientists must be 1 (special case) or between 4 and 18 inclusive")
            End If
        End If

        ' Validate Lab Level (0=None, 1=Basic, 2=Standard, 3=Advanced)
        If tech.Lab < 0 OrElse tech.Lab > 3 Then
            errors.Add("Lab level must be 0 (None), 1 (Basic), 2 (Standard), or 3 (Advanced)")
        End If

        ' Validate Prerequisites count (max 32)
        If tech.Prerequisites.Count > 32 Then
            errors.Add("Maximum of 32 prerequisites allowed (currently " & tech.Prerequisites.Count & ")")
        End If

        ' Validate Unit Properties count (max 32 upgrade tags)
        If tech.UnitProperties.Count > 32 Then
            errors.Add("Maximum of 32 unit properties allowed (currently " & tech.UnitProperties.Count & ")")
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
                ' Validate property value is a valid integer
                Dim propValue As Integer
                If Not Integer.TryParse(prop.Value, propValue) Then
                    errors.Add($"Unit Property value '{prop.Value}' for {prop.UnitType}.{prop.PropertyName} must be a valid integer")
                Else
                    ' Special validation for Armor property (0-5 inclusive)
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

        ' Check maximum tech count (256 max)
        If techList.Count > 256 Then
            errors.Add($"Maximum of 256 technologies allowed (currently {techList.Count})")
        End If

        ' Check for duplicate IDs
        Dim ids As New List(Of String)
        For Each tech In techList.Values
            If ids.Contains(tech.ID) Then
                errors.Add($"Duplicate Tech ID: {tech.ID}")
            Else
                ids.Add(tech.ID)
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

        ' Check for circular dependencies
        For Each tech In techList.Values
            If HasCircularDependency(tech, techList, New List(Of String)) Then
                errors.Add($"Circular dependency detected involving Tech {tech.ID}")
            End If
        Next

        Return errors
    End Function

    Private Shared Function HasCircularDependency(tech As fMain.Technology,
                                                 techList As Dictionary(Of String, fMain.Technology),
                                                 visited As List(Of String)) As Boolean
        If visited.Contains(tech.ID) Then
            Return True
        End If

        visited.Add(tech.ID)

        For Each prereqId In tech.Prerequisites
            If techList.ContainsKey(prereqId) Then
                If HasCircularDependency(techList(prereqId), techList, New List(Of String)(visited)) Then
                    Return True
                End If
            End If
        Next

        Return False
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

        ' Add basic tech levels
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

        ' Add sample technologies
        techList("05999") = GenerateSampleTech()

        Return techList
    End Function
End Class

Public Class TechImportExport
    Public Shared Sub ExportToCSV(techList As Dictionary(Of String, fMain.Technology), fileName As String)
        Try
            Using writer As New StreamWriter(fileName)
                ' Header
                writer.WriteLine("ID,Name,Category,EdenCost,PlymouthCost,MaxScientists,Lab,Teaser,Description,ImproveDesc,Prerequisites,UnitProperties")

                ' Sort technologies by ID
                Dim sortedTechs As New List(Of fMain.Technology)
                For Each tech In techList.Values
                    sortedTechs.Add(tech)
                Next

                ' Simple bubble sort by ID
                For i As Integer = 0 To sortedTechs.Count - 2
                    For j As Integer = 0 To sortedTechs.Count - 2 - i
                        If String.Compare(sortedTechs(j).ID, sortedTechs(j + 1).ID) > 0 Then
                            Dim temp As fMain.Technology = sortedTechs(j)
                            sortedTechs(j) = sortedTechs(j + 1)
                            sortedTechs(j + 1) = temp
                        End If
                    Next
                Next

                For Each tech In sortedTechs
                    ' Build prerequisites string
                    Dim prereqString As String = ""
                    For Each prereq In tech.Prerequisites
                        If prereqString <> "" Then prereqString += ";"
                        prereqString += prereq
                    Next

                    ' Build unit properties string
                    Dim unitPropString As String = ""
                    For Each prop In tech.UnitProperties
                        If unitPropString <> "" Then unitPropString += ";"
                        unitPropString += prop.UnitType + ":" + prop.PropertyName + ":" + prop.Value
                    Next

                    ' Escape quotes in text fields
                    Dim safeName As String = tech.Name.Replace("""", """""")
                    Dim safeTeaser As String = tech.Teaser.Replace("""", """""")
                    Dim safeDescription As String = tech.Description.Replace("""", """""")
                    Dim safeImproveDesc As String = tech.ImproveDesc.Replace("""", """""")

                    ' Build CSV line
                    Dim line As String = String.Format("""{0}"",""{1}"",{2},{3},{4},{5},{6},""{7}"",""{8}"",""{9}"",""{10}"",""{11}""",
                        tech.ID,
                        safeName,
                        tech.Category.ToString(),
                        tech.EdenCost.ToString(),
                        tech.PlymouthCost.ToString(),
                        tech.MaxScientists.ToString(),
                        tech.Lab.ToString(),
                        safeTeaser,
                        safeDescription,
                        safeImproveDesc,
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

        ' Basic counts
        Dim totalTechs As Integer = techList.Count
        Dim edenOnlyCount As Integer = 0
        Dim plymouthOnlyCount As Integer = 0
        Dim bothFactionsCount As Integer = 0
        Dim freeTechsCount As Integer = 0
        Dim researchableTechsCount As Integer = 0

        ' Cost totals
        Dim totalEdenCost As Integer = 0
        Dim totalPlymouthCost As Integer = 0

        ' Category breakdown
        Dim categoryBreakdown As New Dictionary(Of Integer, Integer)

        ' Process each technology
        For Each tech In techList.Values
            ' Faction availability counts
            Dim isEdenAvailable As Boolean = (tech.EdenCost >= 0)
            Dim isPlymouthAvailable As Boolean = (tech.PlymouthCost >= 0)

            If isEdenAvailable And isPlymouthAvailable Then
                bothFactionsCount += 1
            ElseIf isEdenAvailable Then
                edenOnlyCount += 1
            ElseIf isPlymouthAvailable Then
                plymouthOnlyCount += 1
            End If

            ' Free vs researchable
            If tech.EdenCost = 0 Or tech.PlymouthCost = 0 Then
                freeTechsCount += 1
            End If

            If tech.EdenCost > 0 Or tech.PlymouthCost > 0 Then
                researchableTechsCount += 1
            End If

            ' Cost totals
            If tech.EdenCost > 0 Then
                totalEdenCost += tech.EdenCost
            End If

            If tech.PlymouthCost > 0 Then
                totalPlymouthCost += tech.PlymouthCost
            End If

            ' Category breakdown
            If categoryBreakdown.ContainsKey(tech.Category) Then
                categoryBreakdown(tech.Category) += 1
            Else
                categoryBreakdown(tech.Category) = 1
            End If
        Next

        ' Store results
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

' Extension methods for better usability
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
        If tech.ID.Length >= 2 Then
            Dim level As Integer
            If Integer.TryParse(tech.ID.Substring(0, 2), level) Then
                Return level
            End If
        End If
        Return 0
    End Function
End Module