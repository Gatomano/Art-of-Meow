﻿using System;
using System.Diagnostics;

namespace Units.Recursos
{
	/// <summary>
	/// Recurso genérico de stats
	/// </summary>
	public class StatRecurso : Recurso
	{
		#region IUpdate implementation

		public override void Update (float gameTime)
		{
			base.Update (gameTime);
			RegenMax (gameTime);
			RegenCurr (gameTime);
		}

		/// <summary>
		/// Realiza la regeneración del valor máximo
		/// </summary>
		protected virtual void RegenMax (float gameTime)
		{
			var diff = Base - Max;
			Max += diff * gameTime;
		}

		/// <summary>
		/// Realiza la regeneración del valor actual.
		/// </summary>
		protected virtual void RegenCurr (float gameTime)
		{
			var diff = Max - Valor;
			Valor += diff * gameTime;
		}

		#endregion

		#region IRecurso implementation

		/// <summary>
		/// Nombre (debe ser único en el manejador de recursos) del recurso
		/// </summary>
		/// <value>The nombre único.</value>
		protected override string GetShortName ()
		{
			return NombreCorto;
		}

		protected override string GetLongName ()
		{
			return NombreLargo;
		}

		protected override string GetUniqueName ()
		{
			return NombreÚnico;
		}

		/// <summary>
		/// Nombre corto
		/// </summary>
		public string NombreCorto { get; set; }

		/// <summary>
		/// Nombre largo
		/// </summary>
		public string NombreLargo { get; set; }

		public string NombreÚnico { get; }

		readonly float [] currMaxNormal = new float[3];

		IParámetroRecurso ValorP { get; }

		/// <summary>
		/// Valor actual del recurso.
		/// </summary>
		/// <value>The valor.</value>
		public override float Valor
		{ 
			get { return ValorP.Valor; } 
			set { ValorP.Valor = Math.Min (value, Max); }
		}

		/// <summary>
		/// Devuelve o establece el máximo actual
		/// </summary>
		/// <value>The max.</value>
		public float Max
		{ 
			get { return currMaxNormal [1]; } 
			set
			{ 
				currMaxNormal [1] = Math.Min (value, Base); 
				currMaxNormal [0] = Math.Min (currMaxNormal [0], currMaxNormal [1]);
			}
		}

		/// <summary>
		/// Devuelve o establece el valor base.
		/// </summary>
		/// <value>The base.</value>
		public float Base
		{ 
			get { return currMaxNormal [2]; } 
			set
			{ 
				currMaxNormal [2] = value;
				currMaxNormal [1] = Math.Min (value, currMaxNormal [1]); 
				currMaxNormal [0] = Math.Min (currMaxNormal [0], currMaxNormal [1]);
			}

		}

		/// <summary>
		/// Devuelve o establece la tasa de recuparación del valor normal respecto al valor máximo
		/// </summary>
		public float TasaRecuperaciónNormal { get; set; }

		/// <summary>
		/// Devuelve o establece la tasa de recuparación del valor máximo respecto al valor base
		/// </summary>
		public float TasaRecuperaciónMax { get; set; }

		#endregion

		public override string ToString ()
		{
			return base.ToString () + string.Format ("{0}/{1}/{2}", Valor, Max, Base);
		}

		/// <param name="nombreÚnico">Nombre único.</param>
		/// <param name="unidad">Unidad.</param>
		public StatRecurso (string nombreÚnico, IUnidad unidad)
			: base (unidad)
		{
			NombreÚnico = nombreÚnico;
			ValorP = new ValorParám (this, "valor");
			Parámetros.Add (ValorP);
		}

		public class ValorParám : IParámetroRecurso
		{
			public void ReceiveExperience (float exp)
			{
				Valor += exp;
				Debug.WriteLine (
					string.Format ("{0} recibe exp {1}.\nNuevo val: {2}", this, exp, Valor),
					"Experiencia");
			}

			public void Update (float gameTime)
			{
			}

			public IRecurso Recurso { get; }

			public string NombreÚnico { get; }

			public float Valor { get; set; }

			public override string ToString ()
			{
				return string.Format (
					"[ValorParám: Recurso={0}, NombreÚnico={1}, Valor={2}]",
					Recurso,
					NombreÚnico,
					Valor);
			}

			public ValorParám (IRecurso rec, string nombre)
			{
				Recurso = rec;
				NombreÚnico = nombre;
			}
		}
	}
}