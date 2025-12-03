using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharedKernel.Extensions;

public static class ResourceExtension
{
    public static Dictionary<string, ResourceResult> ReadResxFile(string filePath)
    {
        try
        {
            return (from elem in XDocument.Load(filePath).Root.Elements("data")
                    select new KeyValuePair<string, ResourceResult>(elem.Attribute("name")?.Value, new ResourceResult(elem.Attribute("name")?.Value, elem.Element("value")?.Value, elem.Element("comment")?.Value))).ToDictionary();
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading the resx file: " + ex.Message);
        }
    }
}