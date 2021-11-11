using GastroTransfer.Models;
using System.Collections.Generic;
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
                    UnitOfMesure = "kg",
                    ProducedItemId = 1,
                    ExternalGroupId = 2,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Karkówka z grilla",
                    UnitOfMesure = "kg",
                    ProducedItemId = 2,
                    ExternalGroupId = 2,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Wątróbka drobiowa z cebulką",
                    UnitOfMesure = "kg",
                    ProducedItemId = 3,
                    ExternalGroupId = 2,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Pulpety",
                    UnitOfMesure = "kg",
                    ProducedItemId = 4,
                    ExternalGroupId = 2,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Flaki",
                    UnitOfMesure = "l",
                    ProducedItemId = 5,
                    ExternalGroupId = 3,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa gulaszowa",
                    UnitOfMesure = "l",
                    ProducedItemId = 6,
                    ExternalGroupId = 3,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Żurek",
                    UnitOfMesure = "l",
                    ProducedItemId = 7,
                    ExternalGroupId = 3,
                    ConversionRate = 0

                },
                new ProducedItem
                {
                    IsActive = true,
                    Name = "Zupa grochowa",
                    UnitOfMesure = "l",
                    ProducedItemId = 8,
                    ExternalGroupId = 3,
                    ConversionRate = 0

                }
        };

        public static readonly List<ProductGroup> productGroups = new List<ProductGroup>()
            {
                new ProductGroup
                {
                    GroupName = "Wszystkie",
                    ExternalGroupId = 0,
                    ProductGroupId = 1,
                }
        };

    }
}
