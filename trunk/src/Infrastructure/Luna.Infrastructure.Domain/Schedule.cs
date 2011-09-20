using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Iesi.Collections.Generic;
using Luna.Common;

namespace Luna.Infrastructure.Domain
{
    [DebuggerDisplay("{Campaign.Name} : {Start} ~ {End}")]
    public class Schedule : Entity, ISchedule
    {
        public virtual Entity Campaign { get; set; }

        public virtual DateTime Start { get; set; }

        public virtual DateTime End { get; set; }

        

        private Progress _progress = Progress.Unspecified;

        private IDictionary<ServiceQueue, int> _serviceQueues = new Dictionary<ServiceQueue, int>();

        public virtual IDictionary<ServiceQueue, int> ServiceQueues
        {
            get { return _serviceQueues; }
            set { _serviceQueues = value; }
        }

        private Iesi.Collections.Generic.ISet<Entity> _organizations = new HashedSet<Entity>();
        public virtual ICollection<Entity> Organizations
        {
            get { return _organizations; }
            set { _organizations = value as Iesi.Collections.Generic.ISet<Entity>; }
        }

        public virtual Progress Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public virtual int SeatCapacity { get; set; }
        public virtual int MaxSeat { get; set; }

        //todo N+1问题
        private IList<Shrinkage> _shrinkages;
        public virtual IList<Shrinkage> Shrinkages
        {
            get
            {
                if(_shrinkages == null)
                    _shrinkages = new List<Shrinkage>(7);

                if (_shrinkages.Count == 0)
                {
                    for (var i = 0; i < 7; i++)
                    {
                        var entity = new Shrinkage();
                        var list = new List<double>
                                       {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                        entity.Day = (DayOfWeek) Enum.Parse(typeof (DayOfWeek), i.ToString());
                        entity.Elements = new ObservableCollection<double>(list);
                        _shrinkages.Add(entity);
                    }
                }
                return _shrinkages;
            }
            set
            {
                _shrinkages = value;
            }
        }

        public virtual void SetShrinkages()
        {
            foreach (var shrinkage in Shrinkages)
            {
                shrinkage.H0 = shrinkage.Elements[0];
                shrinkage.H1 = shrinkage.Elements[1];
                shrinkage.H2 = shrinkage.Elements[2];
                shrinkage.H3 = shrinkage.Elements[3];
                shrinkage.H4 = shrinkage.Elements[4];
                shrinkage.H5 = shrinkage.Elements[5];
                shrinkage.H6 = shrinkage.Elements[6];
                shrinkage.H7 = shrinkage.Elements[7];
                shrinkage.H8 = shrinkage.Elements[8];
                shrinkage.H9 = shrinkage.Elements[9];
                shrinkage.H10 = shrinkage.Elements[10];
                shrinkage.H11 = shrinkage.Elements[11];
                shrinkage.H12 = shrinkage.Elements[12];
                shrinkage.H13 = shrinkage.Elements[13];
                shrinkage.H14 = shrinkage.Elements[14];
                shrinkage.H15 = shrinkage.Elements[15];
                shrinkage.H16 = shrinkage.Elements[16];
                shrinkage.H17 = shrinkage.Elements[17];
                shrinkage.H18 = shrinkage.Elements[18];
                shrinkage.H19 = shrinkage.Elements[19];
                shrinkage.H20 = shrinkage.Elements[20];
                shrinkage.H21 = shrinkage.Elements[21];
                shrinkage.H22 = shrinkage.Elements[22];
                shrinkage.H23 = shrinkage.Elements[23];
            }
        }
    }

    public class Shrinkage : IEnumerable
    {

        public DayOfWeek Day { get; set; }
        public double H0 { get; set; }
        public double H1 { get; set; }
        public double H2 { get; set; }
        public double H3 { get; set; }
        public double H4 { get; set; }
        public double H5 { get; set; }
        public double H6 { get; set; }
        public double H7 { get; set; }
        public double H8 { get; set; }
        public double H9 { get; set; }
        public double H10 { get; set; }
        public double H11 { get; set; }
        public double H12 { get; set; }
        public double H13 { get; set; }
        public double H14 { get; set; }
        public double H15 { get; set; }
        public double H16 { get; set; }
        public double H17 { get; set; }
        public double H18 { get; set; }
        public double H19 { get; set; }
        public double H20 { get; set; }
        public double H21 { get; set; }
        public double H22 { get; set; }
        public double H23 { get; set; }


        public double[] AsArray()
        {
            return new[]
                       {
                           H0 / 100d,
                           H1 / 100d,
                           H2 / 100d,
                           H3 / 100d,
                           H4 / 100d,
                           H5 / 100d,
                           H6 / 100d,
                           H7 / 100d,
                           H8 / 100d,
                           H9 / 100d,
                           H10 / 100d,
                           H11 / 100d,
                           H12 / 100d,
                           H13 / 100d,
                           H14 / 100d,
                           H15 / 100d,
                           H16 / 100d,
                           H17 / 100d,
                           H18 / 100d,
                           H19 / 100d,
                           H20 / 100d,
                           H21 / 100d,
                           H22 / 100d,
                           H23 / 100d
                       };
        }

        private IList<double> _elements;
        public IList<double> Elements
        {
            get
            {
                if (_elements == null)
                {
                    _elements = new ObservableCollection<double>
                                    {
                                        H0,
                                        H1,
                                        H2,
                                        H3,
                                        H4,
                                        H5,
                                        H6,
                                        H7,
                                        H8,
                                        H9,
                                        H10,
                                        H11,
                                        H12,
                                        H13,
                                        H14,
                                        H15,
                                        H16,
                                        H17,
                                        H18,
                                        H19,
                                        H20,
                                        H21,
                                        H22,
                                        H23
                                    };
                }


                return _elements;
            }
            set
            {
                _elements = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
