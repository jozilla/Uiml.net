/*
    Uiml.Net: a .Net UIML renderer (http://lumumba.uhasselt.be/kris/research/uiml.net)

	 Copyright (C) 2003-2007  Kris Luyten (kris.luyten@uhasselt.be)
                              Expertise Centre for Digital Media (http://edm.uhasselt.be)
                              Hasselt University

	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public License
	as published by the Free Software Foundation; either version 2.1
	of	the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
  
    Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>
*/

using System;
using System.IO;
using System.Reflection;

namespace Uiml.Utils 
{
    public class Location 
    {
        private const string FRONT_ENDS = "front-ends";
    #if !COMPACT
        private const string EXAMPLES = "examples";
    #else
        private const string EXAMPLES = "uiml.net-cf_examples";
    #endif

        private const string VOCABULARIES = "vocabularies";
        private const string ONLINE_VOCS = "http://research.edm.uhasselt.be/~uiml/vocabularies";
        private const string HARMONIA_VOCS = "http://uiml.org/toolkits";

        private static string uimlFileDir = string.Empty;
        private static string appDir;
        private static string frontEndsDir;
        private static string examplesDir;
        private static string[] vocabularyLocations;

        static Location()
        {
            appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            frontEndsDir = Path.Combine(ApplicationDirectory, FRONT_ENDS);
        #if !COMPACT
            examplesDir = Path.Combine(ApplicationDirectory, EXAMPLES);
        #else
            examplesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), EXAMPLES);
        #endif
            vocabularyLocations = new string[] {
                    // first look in vocabularies/ subdir
                    Path.Combine(ApplicationDirectory, VOCABULARIES),
                    // then in current working dir
                    ApplicationDirectory,
                    // then at our online vocabularies 
                    ONLINE_VOCS,
                    // and finally at Harmonia's online vocabularies (very unlikely)
                    HARMONIA_VOCS
            };
        }

        public static string Transform(string file)
        {
            if (file.StartsWith("uiml://"))
            {
                return Path.Combine(UimlFileDirectory, file.Replace("uiml://", string.Empty));
            }

            return file;
        }

        public static string UimlFileDirectory
        {
            get 
            {
                if (uimlFileDir == string.Empty)
                    return ApplicationDirectory;
                else
                    return uimlFileDir;
            }
            set { uimlFileDir = value; }
        }

        public static string ApplicationDirectory 
        {
            get { return appDir; }
        }

        public static string FrontEndDirectory
        {
            get { return frontEndsDir; }
        }

        public static string ExamplesDirectory
        {
            get { return examplesDir; }
        }

        public static string[] VocabularyLocations
        {
            get { return vocabularyLocations; }
        }
    }
}
