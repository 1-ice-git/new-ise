using NewISE.Models.ModelRest;
using System;
using System.Net;

namespace NewISE.Models.dtObj
{
    public class dtDipendentiRest : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public DipendenteRest GetDipendenteRest(string matricola)
        {
            DipendenteRest dr = new DipendenteRest();

            try
            {
                var client = new RestSharp.RestClient("http://bell.ice.it:5000");
                var req = new RestSharp.RestRequest("api/dipendente", RestSharp.Method.GET);
                req.RequestFormat = RestSharp.DataFormat.Json;
                req.AddParameter("matricola", matricola);

                RestSharp.IRestResponse<RetDipendenteJson> resp = client.Execute<RetDipendenteJson>(req);

                if (resp.StatusCode == HttpStatusCode.BadRequest || resp.StatusCode == HttpStatusCode.InternalServerError || resp.StatusCode == HttpStatusCode.NotFound)
                {
                    return dr;
                }

                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                RetDipendenteJson retDip = deserial.Deserialize<RetDipendenteJson>(resp);

                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (retDip.success == true)
                    {
                        if (retDip.items != null)
                        {
                            using (dtAccount dta = new dtAccount())
                            {
                                dr.matricola = retDip.items.matricola;
                                dr.cognome = retDip.items.cognome;
                                dr.nome = retDip.items.nome;
                                dr.cdf = retDip.items.cdf;
                                dr.dataAssunzione = retDip.items.dataAssunzione;
                                dr.dataCessazione = retDip.items.dataCessazione;
                                dr.indirizzo = retDip.items.indirizzo;
                                dr.cap = retDip.items.cap;
                                dr.citta = retDip.items.citta;
                                dr.provincia = retDip.items.provincia;
                                dr.livello = retDip.items.livello;
                                dr.cdc = retDip.items.cdc;
                                dr.email = retDip.items.email;
                                dr.disabilitato = retDip.items.disabilitato;
                                dr.password = retDip.items.password;
                            }
                        }
                    }
                }

                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}