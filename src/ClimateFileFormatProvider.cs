﻿//  Authors:  Amin Almassian, Robert M. Scheller, John McNabb, Melissa Lucash

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class ClimateFileFormatProvider
    {
        private string format;
        private TemporalGranularity timeStep;
        private List<string> maxTempTriggerWord;
        private List<string> minTempTriggerWord;
        private List<string> precipTriggerWord;
        private List<string> windDirectionTriggerWord;
        private List<string> windSpeedTriggerWord;
        private List<string> windEastingTriggerWord;
        private List<string> windNorthingTriggerWord;
        private List<string> nDepositionTriggerWord;
        private List<string> co2TriggerWord;
        private List<string> relativeHumidityTriggerWord;
        private List<string> maxRHTriggerWord;
        private List<string> minRHTriggerWord;
        private List<string> specificHumidityTriggerWord;
        private List<string> petTriggerWord;
        private List<string> parTriggerWord;
        private List<string> ozoneTriggerWord;
        private List<string> shortWaveRadiationTriggerWord;
        private List<string> temperatureTriggerWord;


        private const double ABS_ZERO = -273.15;
            
        //------
        public TemporalGranularity InputTimeStep { get { return this.timeStep; } }
        public List<string> MaxTempTriggerWord { get { return this.maxTempTriggerWord; } }
        public List<string> MinTempTriggerWord { get { return this.minTempTriggerWord; } }
        public List<string> PrecipTriggerWord { get { return this.precipTriggerWord; } }
        public List<string> WindDirectionTriggerWord { get { return this.windDirectionTriggerWord; } }
        public List<string> WindSpeedTriggerWord { get { return this.windSpeedTriggerWord; } }
        public List<string> WindEastingTriggerWord { get { return this.windEastingTriggerWord; } }
        public List<string> WindNorthingTriggerWord { get { return this.windNorthingTriggerWord; } }
        public List<string> NDepositionTriggerWord { get { return this.nDepositionTriggerWord; } }
        public List<string> CO2TriggerWord { get { return this.co2TriggerWord; } }
        public List<string> RelativeHumidityTriggerWord { get { return this.relativeHumidityTriggerWord; } }
        public List<string> MaxRHTriggerWord { get { return this.maxRHTriggerWord; } }
        public List<string> MinRHTriggerWord { get { return this.minRHTriggerWord; } }
        public List<string> SpecificHumidityTriggerWord { get { return this.specificHumidityTriggerWord; } }
        public List<string> PETTriggerWord { get { return this.petTriggerWord; } }
        public List<string> PARTriggerWord { get { return this.parTriggerWord; } }
        public List<string> OzoneTriggerWord { get { return this.ozoneTriggerWord; } }
        public List<string> ShortWaveRadiationTriggerWord { get { return this.shortWaveRadiationTriggerWord; } }
        public List<string> TemperatureTriggerWord { get { return this.temperatureTriggerWord; } }
        public string SelectedFormat { get { return format; } }
  
        // JM: properties for transformations
        public double PrecipTransformation { get; private set; }
        public double TemperatureTransformation { get; private set; }        
        public double WindSpeedTransformation { get; private set; }
        public double WindDirectionTransformation { get; private set; }        

        //------
        public ClimateFileFormatProvider(string format)
        {
            this.format = format;

            // default trigger words
            this.maxTempTriggerWord = new List<string>() { "maxTemp", "Tmax" };
            this.minTempTriggerWord = new List<string>() { "minTemp", "Tmin"};
            this.precipTriggerWord = new List<string>() { "ppt", "precip", "Prcp" };
            this.windDirectionTriggerWord = new List<string>() { "windDirect", "wd", "winddirection", "wind_from_direction" };
            this.windSpeedTriggerWord = new List<string>() { "windSpeed", "ws", "wind_speed" };
            this.windEastingTriggerWord = new List<string>() { "wind_easting", "easting"};
            this.windNorthingTriggerWord = new List<string>() { "northing" , "wind_northing"};
            this.nDepositionTriggerWord = new List<string>() { "Ndeposition", "Ndep" };
            this.co2TriggerWord = new List<string>() { "CO2", "CO2conc" };
            this.relativeHumidityTriggerWord = new List<string>() { "relative_humidity", "RH" };
            this.maxRHTriggerWord = new List<string>() { "max_relative_humidity", "maxRH" };
            this.minRHTriggerWord = new List<string>() { "min_relative_humidity", "minRH" };
            this.specificHumidityTriggerWord = new List<string>() { "specific_humidity", "SH" };
            this.petTriggerWord = new List<string>() { "pet", "PET","potentialevapotranspiration"};
            this.parTriggerWord = new List<string>() { "PAR", "Light"};
            this.ozoneTriggerWord = new List<string>() { "ozone", "O3" };
            this.shortWaveRadiationTriggerWord = new List<string>() { "shortwave_radiation", "SW_radiation", "SWR"};
            this.temperatureTriggerWord = new List<string>() { "Temp", "Temperature"};

            //IMPORTANT FOR ML:  Need to add these as optional trigger words.
            //this.precipTriggerWord = "Prcp";
            //    this.maxTempTriggerWord = "Tmax";
            //    this.minTempTriggerWord = "Tmin";

            // Transformations used for all formats that have temps in C and precip in mm
            this.PrecipTransformation = 0.1;        // Assumes data are in mm and so it converts the data from mm to cm.  
            this.TemperatureTransformation = 0.0;   // Assumes data are in degrees Celsius so no transformation is needed.
            this.WindSpeedTransformation = 3.6;  //Assumes data are in m/s so it converts the data to km/h
            this.WindDirectionTransformation = 180;  //Assumes data are expressed as the direction the wind comes FROM so it converts it to the direction where wind is blowing TO.
      

            //this.timeStep = ((this.format == "PRISM") ? TemporalGranularity.Monthly : TemporalGranularity.Daily);
            switch (this.format.ToLower())
            {
                case "daily_temp-c_precip-mmday":  //was 'gfdl_a1fi' then ipcc3_daily
                    this.timeStep = TemporalGranularity.Daily;  //temp data are in oC and precip is in mm. CFFP (climate file format provider, L62) converts them to cm.
                    break;

                case "monthly_temp-c_precip-mmmonth":  // was ipcc3_monthly
                    this.timeStep = TemporalGranularity.Monthly;    //temp data are in oC and precip is in mm. CFFP converts them to cm.
                    break;

                case "monthly_temp-k_precip-kgm2sec":               //ipcc5 option
                    this.timeStep = TemporalGranularity.Monthly;
                    this.TemperatureTransformation = ABS_ZERO;      // ipcc5 temp. data are in Kelvin.
                    this.PrecipTransformation = 262974.6;            // ipcc5 precip. data are in kg / m2 / sec -> convert to cm / month 
                    break;

                case "daily_temp-k_precip-kgm2sec":             //ipcc5 option
                    this.timeStep = TemporalGranularity.Daily;
                    this.TemperatureTransformation = ABS_ZERO;      // ipcc5 temp. data are in Kelvin.
                    this.PrecipTransformation = 8640.0;             // ipcc5 precip. data are in kg / m2 / sec -> convert to cm / day
                    break;

                case "monthly_temp-k_precip-mmmonth":               //not currently being used on USGS data portal but it's an option nevertheless
                    this.timeStep = TemporalGranularity.Monthly;    
                    this.TemperatureTransformation = ABS_ZERO;      // Temp. data in Kelvin. Precip in mm.      CFFP converts them to cm.               
                    break;

                case "daily_temp-k_precip-mmday":               // U of Idaho data     
                    this.timeStep = TemporalGranularity.Daily;     //Temp are in Kelvin and precip is in mm.   
                    this.TemperatureTransformation = ABS_ZERO;                           
                    break;    
                default:
                    Climate.TextLog.WriteLine("Error in ClimateFileFormatProvider: the given \"{0}\" file format is not supported.", this.format);
                    throw new ApplicationException("Error in ClimateFileFormatProvider: the given \"" + this.format + "\" file format is not supported.");
                   
            }
        }

       


    }
}
