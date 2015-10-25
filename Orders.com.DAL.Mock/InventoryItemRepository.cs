﻿using AutoMapper;
using Orders.com.Core.DataProxy;
using Orders.com.Core.Domain;
using Orders.com.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Orders.com.Core.Exceptions;

namespace Orders.com.DAL.Mock
{
    public class InventoryItemRepository : IInventoryItemDataProxy
    {
        private object _lockObject = new object();

        public InventoryItemRepository()
        {
            Mapper.CreateMap<InventoryItem, InventoryItem>();
        }

        private static List<InventoryItem> _inventoryItems;

        private static List<InventoryItem> InventoryItems
        {
            get
            {
                if (_inventoryItems == null)
                {
                    _inventoryItems = new List<InventoryItem>()
                    {
                        new InventoryItem() { InventoryItemID = 1, ProductID = 1, QuantityOnHand = 10 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 2, ProductID = 2, QuantityOnHand = 1 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 3, ProductID = 3, QuantityOnHand = 4 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 4, ProductID = 4, QuantityOnHand = 3 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 5, ProductID = 5, QuantityOnHand = 20 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 6, ProductID = 6, QuantityOnHand = 54 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 7, ProductID = 7, QuantityOnHand = 12 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 8, ProductID = 8, QuantityOnHand = 10 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 9, ProductID = 9, QuantityOnHand = 34 , Version = "1" },
                        new InventoryItem() { InventoryItemID = 10, ProductID = 10, QuantityOnHand = 11 , Version = "1" }
                    };
                }
                return _inventoryItems;
            }
        }

        public IEnumerable<InventoryItem> GetAll()
        {
            Debug.WriteLine("Executing EF InventoryItem.GetAll");
            // Simulate a SELECT against a database
            return InventoryItems.Select(Mapper.Map<InventoryItem, InventoryItem>).ToArray();
        }

        public InventoryItem GetByID(long id)
        {
            Debug.WriteLine("Executing EF InventoryItem.GetByID");
            var inventoryItem = InventoryItems.First(c => c.ID == id);
            return Mapper.Map(inventoryItem, new InventoryItem());
        }

        public InventoryItem GetByProduct(long productID)
        {
            Debug.WriteLine("Executing EF InventoryItem.GetByProduct");
            var inventoryItem = InventoryItems.First(c => c.ProductID == productID);
            return Mapper.Map(inventoryItem, new InventoryItem());
        }

        public InventoryItem Insert(InventoryItem entity)
        {
            Debug.WriteLine("INSERTING inventoryItem into database");
            lock (_lockObject)
            {
                var nextID = InventoryItems.Max(c => c.ID) + 1;
                entity.ID = nextID;
                entity.IncrementVersionByOne();
                InventoryItems.Add(Mapper.Map(entity, new InventoryItem()));
                return entity;
            }
        }

        public InventoryItem Update(InventoryItem entity)
        {
            lock (_lockObject)
            {
                var existing = InventoryItems.First(c => c.ID == entity.ID && c.Version == entity.Version);
                entity.IncrementVersionByOne();
                Mapper.Map(entity, existing);
                return entity;
            }
        }

        public void Delete(long id)
        {
            Debug.WriteLine("DELETING inventoryItem in database");
            var inventoryItem = InventoryItems.First(c => c.ID == id);
            InventoryItems.Remove(inventoryItem);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return GetAll();
        }

        public async Task<InventoryItem> GetByIDAsync(long id)
        {
            return GetByID(id);
        }

        public async Task<InventoryItem> GetByProductAsync(long productID)
        {
            return GetByProduct(productID);
        }

        public async Task<InventoryItem> InsertAsync(InventoryItem entity)
        {
            return Insert(entity);
        }

        public async Task<InventoryItem> UpdateAsync(InventoryItem entity)
        {
            return Update(entity);
        }

        public async Task DeleteAsync(long id)
        {
            Delete(id);
        }

        public bool SupportsTransactions
        {
            get { return true; }
        }

        public bool IsLatencyProne
        {
            get { return false; }
        }
    }
}
