using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppMoedas.Data;
using FunctionAppMoedas.Models;
using FunctionAppMoedas.Validators;

namespace FunctionAppMoedas
{
    public class MoedasQueueTrigger
    {
        private readonly CotacoesRepository _repository;

        public MoedasQueueTrigger(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [Function("MoedasQueueTrigger")]
        public void Run([QueueTrigger("moedas", Connection = "AzureWebJobsStorage")] string myQueueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger("MoedasQueueTrigger");
            logger.LogInformation("MoedasQueueTrigger - Iniciando o processamento de mensagem...");

            bool dadosValidos;
            DadosCotacao dadosCotacao = null;
            try
            {
                dadosCotacao = JsonSerializer.Deserialize<DadosCotacao>(myQueueItem,
                    new () { PropertyNameCaseInsensitive = true });
                var resultadoValidator = new DadosCotacaoValidator().Validate(dadosCotacao);
                dadosValidos = resultadoValidator.IsValid;
                if (!dadosValidos)
                    logger.LogError(
                        $"MoedasQueueTrigger - Erros de validação: {resultadoValidator.ToString()}");
            }
            catch
            {
                dadosValidos = false;
                logger.LogError($"MoedasQueueTrigger - Erro ao deserializar dados: {myQueueItem}");
            }
            
            if (dadosValidos)
            {
                _repository.Save(dadosCotacao);
                logger.LogInformation($"MoedasQueueTrigger - Dados processados: {myQueueItem}");
            }
        }
    }
}