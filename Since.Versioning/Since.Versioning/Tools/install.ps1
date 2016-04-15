param($installPath, $toolsPath, $package, $project)
$item = $project.ProjectItems.Item("Properties\\AssemblyVersionInfo.tt")
$item.Properties.Item("BuildAction").Value = [int]0
$item.Properties.Item("CustomTool").Value = "TextTemplatingFileGenerator"
$item.Properties.Item("CopyToOutputDirectory").Value = [int]2
$item.Properties.Item("ItemType").Value = "None"
$item.Properties.Item("BuildAction").Value = [int]0