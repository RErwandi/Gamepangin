using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace Gamepangin
{
    [Serializable]
    public class Slots : TSerializableDictionary<int, Slots.Data>, IGameSave
    {
        [Serializable]
        public struct Data
        {
            public string date;
            public string[] keys;
        }

        public int LatestSlot
        {
            get
            {
                int lastSlot = -1;
                DateTime lastDateTime = DateTime.MinValue;

                foreach (KeyValuePair<int, Data> entry in this)
                {
                    if (!DateTime.TryParse(entry.Value.date, out DateTime dateTime)) continue;
                    if (DateTime.Compare(lastDateTime, dateTime) > 0) continue;

                    lastSlot = entry.Key;
                    lastDateTime = dateTime;
                }

                return lastSlot;
            }
        }

        public void Update(int slot, string[] keys)
        {
            this[slot] = new Data
            {
                date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                keys = keys
            };
        }

        public string SaveId => "slots";
        public bool IsShared => true;
        public Type SaveType => typeof(Slots);
        public object SaveData => this;
        public LoadMode LoadMode => LoadMode.OnLoad;

        public Task OnLoad(object value)
        {
            var slots = value as Slots;
            dictionary = slots?.dictionary ?? new Dictionary<int, Data>();
            
            return Task.FromResult(true);
        }
    }
}