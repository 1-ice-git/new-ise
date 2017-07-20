using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NewISE.DBComuniItalia
{
    public partial class Comuni_old : INotifyPropertyChanged
    {
        private string _nome;
        private string _codice;
        private zona _zona;
        private regione _regione;
        private cm _cm;
        private provincia _provincia;
        private string _sigla;
        private string _codiceCatastale;
        private cap _cap;

        [JsonProperty("nome", Required = Required.Always)]
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codice", Required = Required.Always)]
        public string Codice
        {
            get { return _codice; }
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("zona", Required = Required.Always)]
        public zona Zona
        {
            get { return _zona; }
            set
            {
                if (_zona != value)
                {
                    _zona = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("regione", Required = Required.Always)]
        public regione Regione
        {
            get { return _regione; }
            set
            {
                if (_regione != value)
                {
                    _regione = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("cm", Required = Required.Always)]
        public cm Cm
        {
            get { return _cm; }
            set
            {
                if (_cm != value)
                {
                    _cm = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("provincia", Required = Required.Always)]
        public provincia Provincia
        {
            get { return _provincia; }
            set
            {
                if (_provincia != value)
                {
                    _provincia = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("sigla", Required = Required.Always)]
        public string Sigla
        {
            get { return _sigla; }
            set
            {
                if (_sigla != value)
                {
                    _sigla = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codiceCatastale", Required = Required.Always)]
        public string CodiceCatastale
        {
            get { return _codiceCatastale; }
            set
            {
                if (_codiceCatastale != value)
                {
                    _codiceCatastale = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("cap", Required = Required.Always)]
        public cap Cap
        {
            get { return _cap; }
            set
            {
                if (_cap != value)
                {
                    _cap = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Comuni_old FromJson(string data)
        {
            return JsonConvert.DeserializeObject<Comuni_old>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class zona : INotifyPropertyChanged
    {
        private nome _nome;
        private string _codice;

        [JsonProperty("nome", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public nome Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codice", Required = Required.Always)]
        public string Codice
        {
            get { return _codice; }
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static zona FromJson(string data)
        {
            return JsonConvert.DeserializeObject<zona>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class regione : INotifyPropertyChanged
    {
        private string _nome;
        private string _codice;

        [JsonProperty("nome", Required = Required.Always)]
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codice", Required = Required.Always)]
        public string Codice
        {
            get { return _codice; }
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static regione FromJson(string data)
        {
            return JsonConvert.DeserializeObject<regione>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class cm : INotifyPropertyChanged
    {
        private string _nome;
        private string _codice;

        [JsonProperty("nome", Required = Required.Always)]
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codice", Required = Required.Always)]
        public string Codice
        {
            get { return _codice; }
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static cm FromJson(string data)
        {
            return JsonConvert.DeserializeObject<cm>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class provincia : INotifyPropertyChanged
    {
        private string _nome;
        private string _codice;

        [JsonProperty("nome", Required = Required.Always)]
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("codice", Required = Required.Always)]
        public string Codice
        {
            get { return _codice; }
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static provincia FromJson(string data)
        {
            return JsonConvert.DeserializeObject<provincia>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class cap : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static cap FromJson(string data)
        {
            return JsonConvert.DeserializeObject<cap>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public enum nome
    {
        NordOvest = 1,
        NordEst = 2,
        Centro = 3,
        Sud = 4,
        Isole = 5,
    }
}