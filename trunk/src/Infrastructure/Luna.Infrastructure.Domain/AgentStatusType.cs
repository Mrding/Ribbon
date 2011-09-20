using System;
using System.Collections.Generic;
using System.IO;

namespace Luna.Infrastructure.Domain
{
    /// <summary>
    /// AgentStatus类型
    /// </summary>
    public class AgentStatusType : IEquatable<AgentStatusType>, IEqualityComparer<AgentStatusType>
    {
        #region Fields

        //状态类型的图示
        //private BitmapImage _image;
        private byte[] _imageByte;

        #endregion Fields

        #region Properties

        //警告时间1，值必须大于0
        public virtual int AlertTimeOutSecond
        {
            get;
            set;
        }

        //警告时间2，值必须大于警告时间1
        public virtual int AlertTimeOutSecond2
        {
            get;
            set;
        }

        //public virtual BitmapImage Image
        //{
        //    get
        //    {
        //        if (_image == null && _imageByte != null)
        //            _image = ByteToImage(_imageByte);
        //        return _image;
        //    }
        //    set
        //    {
        //        _image = value;
        //    }
        //}

        public virtual byte[] ImageByte
        {
            get { return _imageByte; }
            set
            {
                //if (value == null) return;
                _imageByte = value;

                //_image = ByteToImage(_imageByte);
            }
        }

        public virtual bool IsLogin
        {
            get;
            set;
        }

        public virtual bool IsLogout
        {
            get;
            set;
        }

        public virtual bool OnService
        {
            get;
            set;
        }

        //状态代号，主键，不可重复
        public virtual string StatusCode
        {
            get;
            set;
        }

        //状态名称，可以重复
        public virtual string StatusName
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            var other = obj as AgentStatusType;
            if (other == null) return false;
            return this.Equals(other);
        }

        public virtual bool Equals(AgentStatusType other)
        {
            if (ReferenceEquals(this, other)) return true;
            return StatusCode.Equals(other.StatusCode);
        }

        public virtual bool Equals(AgentStatusType x, AgentStatusType y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public override int GetHashCode()
        {
            return StatusCode.GetHashCode();
        }

        public virtual int GetHashCode(AgentStatusType obj)
        {
            return obj.GetHashCode(obj);
        }



        #endregion Methods
    }
}