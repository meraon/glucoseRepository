﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService.models
{
    public class Glucose : EntryModel
    {
        public static readonly string VALUE_FORMAT = "#.0";

        public Glucose()
        {
        }

        public Glucose(long timestamp, float value)
        {
            setTimestamp(timestamp);
            setValue(value);
        }

        public Glucose(int id, long timestamp, float value)
        {
            setId(id);
            setTimestamp(timestamp);
            setValue(value);
        }
    }
}
