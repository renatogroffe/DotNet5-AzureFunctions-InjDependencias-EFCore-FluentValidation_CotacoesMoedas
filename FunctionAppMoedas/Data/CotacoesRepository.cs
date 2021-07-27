using System;
using System.Linq;
using FunctionAppMoedas.Models;

namespace FunctionAppMoedas.Data
{
    public class CotacoesRepository
    {
        private readonly MoedasContext _context;

        public CotacoesRepository(MoedasContext context)
        {
            _context = context;
        }

        public void Save(DadosCotacao dadosCotacao)
        {
            var cotacao = _context.Cotacoes
                        .Where(c => c.Sigla == dadosCotacao.Sigla)
                        .FirstOrDefault();
            if (cotacao != null)
            {
                cotacao.UltimaCotacao = DateTime.Now;
                cotacao.Valor = dadosCotacao.Valor;
                cotacao.LocalExecucao = Environment.GetEnvironmentVariable("LocalExecucao");
                _context.SaveChanges();
            }
        }

        public Cotacao[] GetAll()
        {
            return _context.Cotacoes.ToArray();
        }
    }
}