using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EODModelViewer.Models
{
    public class IconItems : IModelObject
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
        public int IconRef { get; set; }

        public static List<IconItems> ParseInventoryIcons(string invenJson)
        {
            var reader = new JsonTextReader(new StringReader(invenJson));
            var items = new List<IconItems>();
            IconItems icon = null;

            while (reader.Read())
            {
                switch (reader.Value)
                {
                    case var _ when reader.TokenType == JsonToken.StartObject:
                        icon = new IconItems();
                        break;
                    case var _ when reader.TokenType == JsonToken.EndObject:
                        if (icon != null)
                        {
                            items.Add(icon);
                        }

                        icon = new IconItems();
                        break;
                    case var _ when reader.TokenType == JsonToken.PropertyName:
                        ParseProperty(reader.Value.ToString(), icon, reader);
                        break;
                }
            }

            return items;
        }

        private static void ParseProperty(string propName, IconItems icon, JsonTextReader reader)
        {
            switch (propName)
            {
                case "ModelId":
                    reader.Read();
                    if (int.TryParse(reader.Value.ToString(), out int modelId))
                    {
                        icon.ModelId = modelId;
                    }
                    break;
                case "Name":
                    reader.Read();
                    icon.Name = reader.Value.ToString();
                    break;

                case "IconRef":
                    reader.Read();
                    if (int.TryParse(reader.Value.ToString(), out int iconRef))
                    {
                        icon.IconRef = iconRef;
                    }
                    break;
            }
        }
    }
}
