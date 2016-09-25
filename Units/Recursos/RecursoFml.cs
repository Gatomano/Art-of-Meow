﻿using System;
using AdvMath;

namespace Units.Recursos
{
	/// <summary>
	/// Un recurso secundario de sólo lectura. 
	/// Su valor está determinado por una <see cref="AdvMath.Fórmula"/>
	/// </summary>
	public class RecursoFml : Recurso
	{
		/// <summary>
		/// Fórmula del recurso
		/// </summary>
		/// <value>The fml.</value>
		public string Fórmula { get { return  Fml.StrFormula; } }

		protected Fórmula Fml { get; }

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
		/// Nombre (debe ser único en el manejador de recursos) del recurso
		/// </summary>
		/// <value>The nombre único.</value>
		public string NombreÚnico { get; }

		/// <summary>
		/// Nombre corto
		/// </summary>
		/// <value>The nombre corto.</value>
		public string NombreCorto { get; set; }

		/// <summary>
		/// Nombre largo
		/// </summary>
		/// <value>The nombre largo.</value>
		public string NombreLargo { get; set; }

		/// <summary>
		/// Valor actual del recurso.
		/// </summary>
		/// <value>The valor.</value>
		public override float Valor
		{
			get { return Fml.Evaluar (); }
			set { throw new InvalidOperationException (); }
		}

		#endregion

		public override string ToString ()
		{
			return base.ToString () + string.Format (
				"[RecursoFml: Fórmula={0}]",
				Fórmula);
		}

		/// <summary>
		/// </summary>
		/// <param name="fml">Fórmula del valor</param>
		/// <param name="unidad">Unidad</param>
		/// <param name="nombre">Nombre único</param>
		public RecursoFml (string fml, IUnidad unidad, string nombre)
			: base (unidad)
		{
			Fml = new RecFml (fml, unidad.Recursos);
			NombreÚnico = nombre;
		}

		class RecFml : Fórmula
		{
			readonly ManejadorRecursos _manRec;

			public override float EvaluarVariable (string nombreVariable)
			{
				var ret = _manRec.ValorRecurso (nombreVariable);
				return ret;
			}

			public RecFml (string s, ManejadorRecursos man)
				: base (s)
			{
				_manRec = man;
			}
		}
	}
}