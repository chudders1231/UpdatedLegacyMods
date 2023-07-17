// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>", Scope = "member", Target = "~F:AutosortLockers.AutosorterFilter.Category")]
[assembly: SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>", Scope = "member", Target = "~F:AutosortLockers.AutosorterFilter.Types")]
[assembly: SuppressMessage("Minor Code Smell", "S2386:Mutable fields should not be \"public static\"", Justification = "<Pending>", Scope = "type", Target = "~T:AutosortLockers.AutosorterList")]
[assembly: SuppressMessage("Critical Code Smell", "S2223:Non-constant static fields should not be visible", Justification = "<Pending>", Scope = "member", Target = "~F:AutosortLockers.AutosorterList.Filters")]
[assembly: SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>", Scope = "member", Target = "~F:AutosortLockers.AutosorterList.Filters")]
[assembly: SuppressMessage("Minor Code Smell", "S3260:Non-derived \"private\" classes and records should be \"sealed\"", Justification = "<Pending>", Scope = "type", Target = "~T:AutosortLockers.AutosorterList.TypeReference")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosorterList.InitializeFilters")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosortTarget.ContainsFilter(AutosortLockers.AutosorterFilter)~System.Boolean")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "type", Target = "~T:AutosortLockers.AutosorterList")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosortTarget.RemoveFilter(AutosortLockers.AutosorterFilter)")]
[assembly: SuppressMessage("Blocker Bug", "S2190:Loops and recursions should not be infinite", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosortLocker.Start~System.Collections.IEnumerator")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosortTarget.HasCategoryFilters~System.Boolean")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>", Scope = "member", Target = "~M:AutosortLockers.AutosortTarget.IsTypeAllowed(TechType)~System.Boolean")]
