using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EODModelViewer.Models;

namespace EODModelViewer
{
    public class ModelService
    {
        private static readonly Lazy<ModelService> Lazy = new Lazy<ModelService>(() => new ModelService());
        private readonly List<Item> _items;
        private readonly List<Mob> _mobs;
        private readonly List<IconItems> _icons;

        public static ModelService Instance => Lazy.Value;

        private ModelService()
        {
            var dataService = new ModelDataService();
            var rawData = dataService.GetData().Result;
            var parsedData = dataService.ParseData(rawData);
            _items = parsedData.Result["items"].Cast<Item>().ToList();
            _mobs = parsedData.Result["mobs"].Cast<Mob>().ToList();
            _icons = parsedData.Result["iconitems"].Cast<IconItems>().ToList();
        }

        public List<Item> GetItems()
        {
            return _items;
        }

        public Item GetItem(int modelId)
        {
            return _items.SingleOrDefault(x => x.ModelId == modelId);
        }

        public List<Mob> GetMobs()
        {
            return _mobs;
        }

        public Mob GetMob(int modelId)
        {
            return _mobs.SingleOrDefault(x => x.ModelId == modelId);
        }

        public List<IconItems> GetInventory()
        {
            return _icons;
        }

        public IconItems GetInventory(int modelId)
        {
            return _icons.SingleOrDefault(x => x.ModelId == modelId);
        }

        public async Task<Image> GetItemPicture(int modelId)
        {
            var path = "./EODModelViewer/models/items/";
            var image = LoadAsset(modelId, path);

            if (image != null)
            {
                return image;
            }

            image = await DownloadAsset(modelId, ImageType.Item);

            if (image == null)
            {
                return null;
            }

            image.Save($"{path}/{modelId}.jpg");
            return image;
        }

        public async Task<Image> GetMobPicture(int modelId)
        {
            var path = "./EODModelViewer/models/mobs/";
            var image = LoadAsset(modelId, path);

            if (image != null)
            {
                return image;
            }

            image = await DownloadAsset(modelId, ImageType.Mob);

            if (image == null)
            {
                return null;
            }

            image.Save($"{path}/{modelId}.jpg");
            return image;
        }

        public async Task<Image> GetIconPicture(int modelId)
        {
            var path = "./EODModelViewer/models/icons/items";
            var image = LoadAsset(modelId, path);

            if (image != null)
            {
                return image;
            }

            image = await DownloadAsset(modelId, ImageType.Icon);

            if (image == null)
            {
                return null;
            }

            image.Save($"{path}/{modelId}.jpg");
            return image;
        }

        private Image LoadAsset(int modelId, string path)
        {
            Directory.CreateDirectory(path);

            var filePath = $"{path}{modelId}.jpg";
            if (File.Exists(filePath))
            {
                return Image.FromFile(filePath);
            }

            return null;
        }

        private enum ImageType
        {
            Item, Mob, Icon
        }

        private async Task<Image> DownloadAsset(int modelId, ImageType type)
        {
            string modelPath = "";

            switch (type)
            {
                case ImageType.Item: modelPath = "items";
                    break;
                case ImageType.Mob: modelPath = "mobs";
                    break;
                case ImageType.Icon: modelPath = "icons/items";
                    break;
            }
            using (var webClient = new WebClient())
            {
                try
                {
                    var url = $"github.com/Eve-of-Darkness/DolModels/raw/master/src/{modelPath}/{modelId}.jpg";
                    var data = await webClient.DownloadDataTaskAsync(new Uri("https://" + url));
                    var image = Image.FromStream(new MemoryStream(data));
                    return image;
                }
                catch (WebException)
                {
                }
            }

            return null;
        }
    }
}
