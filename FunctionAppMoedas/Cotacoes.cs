using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using FunctionAppMoedas.Data;

namespace FunctionAppMoedas
{
    public class Cotacoes
    {
        private readonly CotacoesRepository _repository;

        public Cotacoes(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [Function("Cotacoes")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Cotacoes");
            logger.LogInformation("Cotacoes - Processando requisição HTTP...");

            var dados = _repository.GetAll();
            logger.LogInformation($"Qtde. registros = {dados.Length}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(dados).AsTask().Wait();
            return response;
        }
    }
}