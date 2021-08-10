﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Abstract;

namespace Entities.Concrete
{
    public class Color:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
