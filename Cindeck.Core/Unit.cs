﻿using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    [DataContract(IsReference = true)]
    [ImplementPropertyChanged]
    public class Unit
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot1
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot2
        {
            get;
            set;
        }

        /// <summary>
        /// センターです
        /// </summary>
        [DataMember]
        public OwnedIdol Slot3
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot4
        {
            get;
            set;
        }

        [DataMember]
        public OwnedIdol Slot5
        {
            get;
            set;
        }

        public OwnedIdol Center
        {
            get
            {
                return Slot3;
            }
        }

        [DependsOn("Slot1", "Slot2", "Slot3", "Slot4", "Slot5")]
        public IEnumerable<OwnedIdol> Slots
        {
            get
            {
                yield return Slot1;
                yield return Slot2;
                yield return Slot3;
                yield return Slot4;
                yield return Slot5;
            }
        }

        public int Vocal => Slots.Sum(x => x == null ? 0 : x.Vocal);

        public int Dance => Slots.Sum(x => x == null ? 0 : x.Dance);

        public int Visual => Slots.Sum(x => x == null ? 0 : x.Visual);

        public int Life => Slots.Sum(x => x == null ? 0 : x.Life);

        public int TotalAppeal => Dance + Vocal + Visual;

        public int TotalAppealWithCenterEffect => DanceWithCenterEffect + VocalWithCenterEffect + VisualWithCenterEffect;

        public int VocalWithCenterEffect => Slots.Sum(x => GetVocal(x));

        public int DanceWithCenterEffect => Slots.Sum(x => GetDance(x));

        public int VisualWithCenterEffect => Slots.Sum(x => GetVisual(x));

        public int LifeWithCenterEffect => Slots.Sum(x => GetLife(x));

        private int GetLife(OwnedIdol idol)
        {
            if(idol==null)
            {
                return 0;
            }
            var effect = Center?.CenterEffect as CenterEffect.LifeUp;
            if (effect?.Targets.HasFlag(idol.Category) == true)
            {
                return (int)Math.Ceiling(idol.Life + idol.Life * effect.Rate);
            }
            return idol.Life;
        }

        private int GetVocal(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            var effect = Center?.CenterEffect as CenterEffect.AppealUp;
            if (effect?.Targets.HasFlag(idol.Category) == true && effect?.TargetAppeal.HasFlag(AppealType.Vocal) == true)
            {
                return (int)Math.Ceiling(idol.Vocal + idol.Vocal * effect.Rate);
            }
            return idol.Vocal;
        }

        private int GetDance(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            var effect = Center?.CenterEffect as CenterEffect.AppealUp;
            if (effect?.Targets.HasFlag(idol.Category) == true && effect?.TargetAppeal.HasFlag(AppealType.Dance) == true)
            {
                return (int)Math.Ceiling(idol.Dance + idol.Dance * effect.Rate);
            }
            return idol.Dance;
        }

        private int GetVisual(OwnedIdol idol)
        {
            if (idol == null)
            {
                return 0;
            }
            var effect = Center?.CenterEffect as CenterEffect.AppealUp;
            if (effect?.Targets.HasFlag(idol.Category) == true && effect?.TargetAppeal.HasFlag(AppealType.Visual) == true)
            {
                return (int)Math.Ceiling(idol.Visual + idol.Visual * effect.Rate);
            }
            return idol.Visual;
        }

        public bool AlreadyInUnit(OwnedIdol idol)
        {
            return Slots.Any(x => x?.Iid == idol.Iid);
        }

        public bool OccupiedByUnit(OwnedIdol idol)
        {
            return Slots.Any(x => x == idol);
        }

        public void RemoveIdol(OwnedIdol idol)
        {
            if (Slot1 == idol) Slot1 = null;
            if (Slot2 == idol) Slot2 = null;
            if (Slot3 == idol) Slot3 = null;
            if (Slot4 == idol) Slot4 = null;
            if (Slot5 == idol) Slot5 = null;
        }

        public Unit Clone()
        {
            return new Unit { Name = Name, Slot1 = Slot1, Slot2 = Slot2, Slot3 = Slot3, Slot4 = Slot4, Slot5 = Slot5 };
        }

        public void CopyFrom(Unit unit)
        {
            Name = unit.Name;
            Slot1 = unit.Slot1;
            Slot2 = unit.Slot2;
            Slot3 = unit.Slot3;
            Slot4 = unit.Slot4;
            Slot5 = unit.Slot5;
        }
    }
}
