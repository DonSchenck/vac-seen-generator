using System;
using System.IO;
using System.Collections.Generic;

public class ServiceBinding {
  
  // var tupleList = new List<(string Firstname, string Lastname)>;

  public List<(string key,string value)> Main (string[] args) {

    var bindingDirectory = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT");
    return ProcessDirectoryTree(bindingDirectory);
  }


  public static List<(string key,string value)> ProcessDirectoryTree(string directory)
  {
    // Walk down the directory tree and for each file, use the filename
    // as the key and the contents as the value, and add this
    // key-value pair to a list of key-value pairs.
    // At the end, return the list.

    List<(string key,string value)> l = new List<(string key,string value)>();

    foreach (string f in Directory.GetFiles(directory))
    {
        var r = GetFileContents(Path.GetFileName(f));
        l.Add(r);
    } 

    foreach (string d in Directory.GetDirectories(directory))
    {
      ProcessDirectoryTree(d);
    }
    return l;
  }
  private static (string key, string value) GetFileContents(string filename)
  {
    // Get contents of file
    string v = System.IO.File.ReadAllText(filename);
    return (filename, v);
  }
}