﻿using GalaSoft.MvvmLight;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FocusTreeManager.Model.LegacySerialization
{
    [ProtoContract(SkipConstructor = true)]
    public class MutuallyExclusiveSet : ObservableObject, ISet
    {
        [ProtoMember(1, AsReference = true)]
        private Focus focus1;

        public Focus Focus1
        {
            get
            {
                return focus1;
            }
            set
            {
                Set<Focus>(() => Focus1, ref focus1, value);
            }
        }

        [ProtoMember(2, AsReference = true)]
        private Focus focus2;

        public Focus Focus2
        {
            get
            {
                return focus2;
            }
            set
            {
                Set<Focus>(() => Focus2, ref focus2, value);
            }
        }

        public MutuallyExclusiveSet(Focus Focus1, Focus Focus2)
        {
            //Set leftmost Focus as Focus 1 and rightmost focus as focus 2
            if (Focus1.X < Focus2.X)
            {
                this.Focus1 = Focus1;
                this.Focus2 = Focus2;
            }
            else if(Focus1.X >= Focus2.X)
            {
                this.Focus2 = Focus1;
                this.Focus1 = Focus2;
            }
        }

        public void assertInternalFocus(IEnumerable<Focus> fociList)
        {
            //Repair Focus 1, get the reference in the list
            Focus1 = fociList.FirstOrDefault((f) => f.X == Focus1.X && f.Y == Focus1.Y);
            //Repair Focus 2, get the reference in the list
            Focus2 = fociList.FirstOrDefault((f) => f.X == Focus2.X && f.Y == Focus2.Y);
        }

        public void DeleteSetRelations()
        {
            throw new NotImplementedException();
        }
    }
}
