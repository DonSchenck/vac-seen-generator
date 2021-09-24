using System;
using System.IO;

public class ServiceBinding {
  public static List<Tuple> Main (string[] args) {

    var bindingDirectory = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT");
    return ProcessDirectoryTree(bindingDirectory);
  }


  public static List<Tuple> ProcessDirectoryTree(string directory)
  {
    // Walk down the directory tree and for each file, use the filename
    // as the key and the contents as the value, and add this
    // key-value pair to a list of key-value pairs.
    // At the end, return the list.

    List<Tuple> l();

    foreach (string f in Directory.GetFiles(directory))
    {
        var r = GetFileContents(Path.GetFileName(f),"");
        l.add(r.key,r.value);
    } 

    foreach (string d in Directory.GetDirectories(directory))
    {
      ProcessDirectoryTree(d);
    }
  }
  (string key, string value) GetFileContents(string filename)
  {
    // Get contents of file
    string v = System.IO.File.ReadAllText(filename);
    return (filename, v);
  }
}