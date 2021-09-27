using System;
using System.IO;
using System.Collections.Generic;

public class ServiceBinding {
  
  // var tupleList = new List<(string Firstname, string Lastname)>;

  public List<KeyValuePair<string,string>> GetBindings () {

    var bindingDirectory = Environment.GetEnvironmentVariable("SERVICE_BINDING_ROOT");
    Console.WriteLine("Searching directory " + bindingDirectory);
    return ProcessDirectoryTree(bindingDirectory);
  }

  public static List<KeyValuePair<string,string>> ProcessDirectoryTree(string directory)
  {
    // Walk down the directory tree and for each file, use the filename
    // as the key and the contents as the value, and add this
    // key-value pair to a list of key-value pairs.
    // At the end, return the list.

    List<KeyValuePair<string,string>> l = new List<KeyValuePair<string,string>>();

    foreach (string f in Directory.GetFiles(directory))
    {
        l.Add(GetFileContents(f));
    } 

    foreach (string d in Directory.GetDirectories(directory))
    {
      ProcessDirectoryTree(d);
    }
    return l;
  }
  private static KeyValuePair<string,string> GetFileContents(string filename)
  {
    // Get contents of file
    string value = System.IO.File.ReadAllText(filename);
    string key = Path.GetFileName(filename);
    return new KeyValuePair<string,string>(key,value);
  }
}