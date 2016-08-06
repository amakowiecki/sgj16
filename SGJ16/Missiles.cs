using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Missiles : IEnumerable<Missile>
    {
        private class MissileInfo
        {
            public Missile Missile;
            public bool IsUsed;
        }

        public Map Map { get; private set; }

        private List<MissileInfo> missiles;
        public Dictionary<MissileModelType, MissileModel> Models;

        public Missiles(Map map, int capacity)
        {
            Map = map;
            Models = new Dictionary<MissileModelType, MissileModel>();
            missiles = new List<MissileInfo>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                AddEmpty(i);
            }
        }

        public int Count { get { return missiles.Count; } }

        private void AddEmpty(int idx)
        {
            missiles.Add(new MissileInfo
            {
                Missile = new Missile(this, idx),
                IsUsed = false
            });
        }

        public MissileModel GetMissileModel(MissileModelType modelType)
        {
            if (Models.ContainsKey(modelType))
            {
                return Models[modelType];
            }
            return null;
        }

        public void InitializeMissile(MissileModelType modelType, Vector2 initialPosition, Vector2 velocity)
        {
            int index = FirstUnusedIndex();
            if (index >= missiles.Count)
            {
                index = missiles.Count;
                AddEmpty(index);
            }
            InitializeMissileAt(index, modelType, initialPosition, velocity);
        }

        public void DisposeMissile(Missile missile)
        {
            missiles[missile.collectionIdx].IsUsed = false;
        }

        private int FirstUnusedIndex()
        {
            int i = 0;
            for (; i < missiles.Count; i++)
            {
                if (!missiles[i].IsUsed)
                {
                    return i;
                }
            }
            return i;
        }

        private void InitializeMissileAt(int idx, MissileModelType modelType,
            Vector2 initialPosition, Vector2 velocity)
        {
            Missile m = missiles[idx].Missile;
            missiles[idx].IsUsed = true;
            m.Initialize(modelType, initialPosition, velocity);
        }

        public IEnumerator<Missile> GetEnumerator()
        {
            return new MissilesEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class MissilesEnumerator : IEnumerator<Missile>
        {
            Missiles enumerable;
            int current;

            public MissilesEnumerator(Missiles enumerable)
            {
                this.enumerable = enumerable;
                current = -1;
            }

            public Missile Current
            {
                get { return current < enumerable.Count ? enumerable.missiles[current].Missile : null; }
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
                enumerable = null;
            }

            private int GetFirtUsedIndexFrom(int index)
            {
                while (++index < enumerable.Count)
                if (enumerable.missiles[index].IsUsed) { return index; }
                return index;
            }

            public bool MoveNext()
            {
                current = GetFirtUsedIndexFrom(current);
                return current < enumerable.Count;
            }

            public void Reset()
            {
                current = 0;
            }
        }
    }

}
