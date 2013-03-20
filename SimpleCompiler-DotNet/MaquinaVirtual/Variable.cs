using System;
using CompiladorReducido.Compartido;

namespace CompiladorReducido.MaquinaVirtual
{
	public class Variable
	{
		public TipoDato	Tipo;
		object valor;

		public Variable()
		{
		}

		private void ValidarTipoOSetear(TipoDato.TipoDatoEnum tipo)
		{
			if (Tipo.Tipo != TipoDato.TipoDatoEnum.TIPO_NINGUNO)
			{
                if (Tipo.Tipo != tipo)
                    throw new ErrorEjecucion("Tipo de variable incorrecto");
			}
			else
				Tipo.Tipo = tipo;
		}

		private void ValidarTipo(TipoDato.TipoDatoEnum tipo)
		{
			if (Tipo.Tipo != tipo)
				throw new ErrorEjecucion("Tipo de variable incorrecto");
		}

		public void SetearInt(int valor)
		{
			ValidarTipoOSetear(TipoDato.TipoDatoEnum.TIPO_INT);

			this.valor = valor;
		}

		public void SetearFloat(float valor)
		{
			ValidarTipoOSetear(TipoDato.TipoDatoEnum.TIPO_FLOAT);

			this.valor = valor;
		}

		public void SetearString(string valor)
		{
			ValidarTipoOSetear(TipoDato.TipoDatoEnum.TIPO_STRING);

			this.valor = valor;
		}

		public int ObtenerInt()
		{
			ValidarTipo(TipoDato.TipoDatoEnum.TIPO_INT);

			return (int) valor;
		}

		public float ObtenerFloat()
		{
			ValidarTipo(TipoDato.TipoDatoEnum.TIPO_FLOAT);

			return (float) valor;
		}

		public string ObtenerString()
		{
			ValidarTipo(TipoDato.TipoDatoEnum.TIPO_STRING);

			return (string) valor;
		}
	}
}
