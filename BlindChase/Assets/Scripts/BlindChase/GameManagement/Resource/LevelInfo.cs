﻿using System;
using System.Collections.Generic;

namespace BlindChase.GameManagement
{
    public static class LevelInfo 
    {
        public static List<Chapter> Chapters = new List<Chapter>
        { 

        
        };
    }

    public class Chapter
    {
        public List<Page> Pages = new List<Page> 
        { 
        
        };
    }

    public class Page 
    { 
        public string PageId { get; private set; } 
    }

}


