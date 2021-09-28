using System;
using System.IO;
using System.Collections.Generic;

public class ServiceBinding {
  
  private static Dictionary<string,string> _l;

  public Dictionary<string,string> GetBindings (string type) {
    var bindingDirectory = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT");
    Console.WriteLine("Searching directory " + bindingDirectory);
    _l = new Dictionary<string,string>();
    ProcessDirectoryTree(bindingDirectory, type);
    return _l;
  }

  private static void ProcessDirectoryTree(string directory, string type)
  {
    // Walk down the directory tree and for each file, use the filename
    // as the key and the contents as the value, and add this
    // key-value pair to a list of key-value pairs.
    // At the end, return the list.

    if (File.Exists(directory + "/type") && System.IO.File.ReadAllText(directory + "/type") == type) {
      foreach (string f in Directory.GetFiles(directory))
        {
          GetFileContents(f);
        } 
    }

    foreach (string d in Directory.GetDirectories(directory))
    {
      ProcessDirectoryTree(d, type);
    }
  }

  private static void GetFileContents(string filename)
  {
    // Get contents of file
    string value = System.IO.File.ReadAllText(filename);
    string key = Path.GetFileName(filename);
    Dictionary<string,string> d = new Dictionary<string,string>();
    _l.Add(key, value);
  }
}