using Newtonsoft.Json.Linq;
using SpillTracker.Models.Interfaces;
using SpillTracker.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpillTracker.Services;

namespace SpillTracker.Models
{
    public class PugAPI : IPugAPI
    {
        private readonly IAPICall _apiCall;
        public PugAPI( IAPICall apiCall)
        {
            /* _context = context;*/
            _apiCall = apiCall;
        }
        public double ParseDensityFromJSON(string jsonString)
        {
            //The API call was successful populate the density and vapor pressure 
            JObject geo = JObject.Parse(jsonString);

            string DensityString = (string)geo["Record"]["Section"][0]["Section"][0]["Section"][0]["Information"][0]["Value"]["StringWithMarkup"][0]["String"];
            double Density = RegexParserUtilities.RegexDensityParse(DensityString);
            return Density;
        }

        public double ParseVaporPresureFromJSON(string jsonString)
        {
            /* Debug.WriteLine(jsonString);*/
            //The call was successful populate the vapor pressure 
            JObject geo = JObject.Parse(jsonString);

            //System.Text.RegularExpressions.Regex  
            string vaporPressureString = (string)geo["Record"]["Section"][0]["Section"][0]["Section"][0]["Information"][0]["Value"]["StringWithMarkup"][0]["String"];
            return RegexParserUtilities.RegexVaporParse(vaporPressureString); 
        }

        public ExtraChemData GitCIDAndMolWeightFromPugRest(string url)
        {
            ExtraChemData cIDMol = new ExtraChemData();
            string jsonString = null;
            //try to search for compound using the CID from Pug-Rest API If that doesn't work try again with CAS-prepended to the CAS number
            try
            {
                 jsonString =  _apiCall.DoAPICall(url);            
            }
            catch (Exception)
            {
                //The normal CAS format failed to return a result try using alternate format, prepend CAS- to the CAS number
                url = url.Insert(56, "CAS-");
                try
                {
                    jsonString = _apiCall.DoAPICall(url);
                }
                catch (Exception)
                {
                    //Alternate format did not return any results return null.
                    Debug.WriteLine("standard and alternate formats did not return results.");
                }
            }

            if(jsonString != null)
            {
                //The API call worked correctly now update the cid and molecular weight
                /* Debug.WriteLine(jsonString);*/
                JObject geo = JObject.Parse(jsonString);

                cIDMol.CID = (int)geo["PropertyTable"]["Properties"][0]["CID"];
                cIDMol.MolecularWeight = (double)geo["PropertyTable"]["Properties"][0]["MolecularWeight"];
            }

            return cIDMol;      
        }

        public ExtraChemData GetDensVapPresFromPUGView(ExtraChemData data)
        {
            string jsonString;
            string densityURL = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/{data.CID}/JSON?heading=Density";
            string vaporURL = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/{data.CID}/JSON?heading=Vapor+Pressure"; 
  
            //try to send an API call to the PugView API for Density and Vapor Pressure
            try
            {
                jsonString = _apiCall.DoAPICall(densityURL);
                data.Density = ParseDensityFromJSON(jsonString);
            }
            catch (Exception)
            {
                //API call failed no density found
                data.Density = -1;
                Debug.WriteLine("density not found");
            }

            //try to send an API call to the PugView API for Vapor Pressure
            try
            {
                jsonString = _apiCall.DoAPICall(vaporURL);
                data.VaporPressure = ParseVaporPresureFromJSON(jsonString);
            }
            catch (Exception)
            {
                //API call failed set the vapor pressure to 0
                data.VaporPressure = -1;
                Debug.WriteLine("vapor Pressure not found");
            }

            return (data);
        }

    }
}
