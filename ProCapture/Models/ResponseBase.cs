using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCapture.Models
{
    public class ResponseBase
    {
        public ResponseBase(bool result)
        {
            IsSuccess = result;
        }

        /// <summary>
        /// Shows if Action was done Successfully.
        /// </summary>
        public bool IsSuccess { get; set; }
    }

    public class ResponseBase<TEntity>
    {

        public ResponseBase(bool result, TEntity entity)
        {
            IsSuccess = result;
            Entity = entity;
        }

        /// <summary>
        /// Shows if Action was done Successfully.
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// The Entity Of Response.
        /// </summary>
        public TEntity Entity { get; set; }
    }
}
