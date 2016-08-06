using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public enum HpRangeType
    {
        Normal,
        Low,
        VeryLow
    }

    public struct HpRange
    {
        public float UpperBound;
        public HpRangeType Type;
    }

    public class HpBar
    {
        private static List<HpRange> ranges;
        public static Dictionary<HpRangeType, Texture2D> Textures;
        public static Texture2D BackTexture;

        public static List<HpRange> Ranges
        {
            get { return ranges; }
            set
            {
                value.Sort((r1, r2) => { return r1.UpperBound.CompareTo(r2.UpperBound); });
                ranges = value;
            }
        }

        public Player Player;
        public Vector2 Position;

        static HpBar()
        {
            ranges = new List<HpRange>();
            Textures = new Dictionary<HpRangeType, Texture2D>();
        }

        public HpBar(Player player)
        {
            Player = player;
            player.HpBar = this;
            visibleHpPerc = player.CurrentHp / player.MaxHp;
        }

        public Texture2D Texture
        {
            get
            {
                float perc = visibleHpPerc;
                int idx = 0;
                int n = Textures.Count - 1;
                while (idx < n && Ranges[idx].UpperBound <= perc)
                {
                    ++idx;
                }
                return Textures[Ranges[idx].Type];
            }
        }

        public void Update()
        {
            float cPerc = currentHpPerc;
            float percStep = Config.HP_BAR_PERC_STEP / 100;
            if (visibleHpPerc > cPerc)
            {
                if (visibleHpPerc - cPerc > percStep)
                { visibleHpPerc -= percStep; }
                else
                { visibleHpPerc = cPerc; }
            }
            if (visibleHpPerc < cPerc)
            {
                if (visibleHpPerc < cPerc - percStep)
                { visibleHpPerc += percStep; }
                else
                { visibleHpPerc = cPerc; }
            }
            if (visibleHpPerc < 0) { visibleHpPerc = 0; }
            else if (visibleHpPerc > 1) { visibleHpPerc = 1; }
        }

        private float visibleHpPerc;
        private float currentHpPerc { get { return Player.CurrentHp / Player.MaxHp; } }

        public float VisibleHpPercentage { get { return visibleHpPerc; } }
        
    }
}
