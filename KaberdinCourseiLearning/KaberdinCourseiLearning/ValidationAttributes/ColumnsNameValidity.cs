using KaberdinCourseiLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.ValidationAttributes
{
    public class ColumnsNameValidity : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var columns = value as ProductCollectionColumn[];
            foreach(var column in columns)
            {
                if (string.IsNullOrWhiteSpace(column.ColumnName)) return false;
            }
            return true;
        }
    }
}
