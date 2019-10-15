using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GastroTransfer.Data
{
    class ConstData
    {
        public static readonly List<Key> numpadKeys = new List<Key>()
        {
            Key.D0, Key.D1, Key.D2,
            Key.D3, Key.D4, Key.D5,
            Key.D6, Key.D7, Key.D8,
            Key.D9, Key.Decimal, Key.Return,
            Key.NumPad0, Key.NumPad1, Key.NumPad2,
            Key.NumPad3, Key.NumPad4, Key.NumPad5,
            Key.NumPad6, Key.NumPad7, Key.NumPad8,
            Key.NumPad9, Key.OemComma, Key.Enter,
            Key.Escape, Key.Tab
        };
        public static readonly List<ProducedItem> producedItems = new List<ProducedItem>()
            {
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Schab w sosie grzybowym",
                    ProducedItemId = 1,
                    ProductGroupId = 2

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Karkówka z grilla",
                    ProducedItemId = 2,
                    ProductGroupId = 2

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Flaki",
                    ProducedItemId = 3,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa gulaszowa",
                    ProducedItemId = 4,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Żurek",
                    ProducedItemId = 5,
                    ProductGroupId = 3

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa grochowa",
                    ProducedItemId = 6,
                    ProductGroupId = 3

                }
        };

        public static readonly List<ProductGroup> productGroups = new List<ProductGroup>()
            {
                new ProductGroup
                {
                    GroupName = "Wszystkie",
                    ProductGroupId = 1,

                },
                new ProductGroup
                {
                    GroupName = "Zupy",
                    ProductGroupId = 3,

                },
                new ProductGroup
                {
                    GroupName = "Dania",
                    ProductGroupId = 2,

                },
                new ProductGroup
                {
                    GroupName= "Śniadania",
                    ProductGroupId = 4,

                }
        };
    }
}
