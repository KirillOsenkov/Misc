System.Windows.Forms.Clipboard.SetText(
  string.Join("\r\n", 
  typeof(Environment.SpecialFolder).GetFields()
    .Where(f => f.FieldType == typeof(Environment.SpecialFolder))
    .Select(f => f.Name + " = " + Environment.GetFolderPath((Environment.SpecialFolder)f.GetValue(null)))
    .ToArray()))
