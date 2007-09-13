using System;
using System.Reflection;
using System.IO;

namespace Uiml.Utils.Reflection
{
	public class AssemblyLoader
	{
		class AssemblyQuery
		{
			private string m_query;
			
			public AssemblyQuery(string query)
			{
				m_query = query;
			}

			public AssemblyQuery(AssemblyQuery other)
			{
				m_query = other.m_query;
			}

			public AssemblyQuery ToPartialName()
			{
				if (IsPartialName)
				{
                    return this;
				}
				else
				{
					// cut off assembly extension and path
					string partialName = Path.GetFileNameWithoutExtension(Query);

					return new AssemblyQuery(partialName);
				}
			}

			public AssemblyQuery ToPath()
			{
                if (IsPath)
                    return this;
                else
                    return new AssemblyQuery(Query + ASSEMBLY_EXTENSION);
			}

            public AssemblyName ToAssemblyName() 
            {
                AssemblyName an = new AssemblyName();
                an.Name = this.ToPartialName().Query;

                return an;
            }

			public string Query
			{
				get { return m_query; }
			}

			public bool IsPath
			{
				get { return m_query.EndsWith(ASSEMBLY_EXTENSION); }
			}

			public bool IsPartialName
			{
				get { return !IsPath; }
			}

			public const string ASSEMBLY_EXTENSION = ".dll";
		}


		/// <summary>
		/// Loads an assembly from a specified file.
		/// </summary>
		/// <param name="pathToLib">the path to the assembly</param>
		/// <returns>the loaded assembly</returns>
		public static Assembly LoadFromPath(string pathToLib)
		{
			return LoadFromPath(new AssemblyQuery(pathToLib));
		}

		private static Assembly LoadFromPath(AssemblyQuery q)
		{
			if (!q.IsPath)
				throw new AssemblyNotFoundException(
					q.Query
					+ " is not a valid assembly file (it should end with " 
					+ AssemblyQuery.ASSEMBLY_EXTENSION + ")"
				);

			try
			{
				return Assembly.LoadFrom(q.Query);
			}
			catch (Exception e)
			{
				throw new AssemblyNotFoundException(q.Query, e);
			}
		}

		/// <summary>
		/// Loads an assembly from the Global Assembly Cache (GAC) or 
		/// from the current application directory.
		/// </summary>
		/// <param name="partialName">the assembly's partial name 
		/// (e.g. "System.Drawing")</param>
		/// <returns>the loaded assembly</returns>
		public static Assembly LoadFromGacOrAppDir(string partialName)
		{
			return LoadFromGacOrAppDir(new AssemblyQuery(partialName));
		}

		private static Assembly LoadFromGacOrAppDir(AssemblyQuery q)
		{
			if (!q.IsPartialName)
				throw new AssemblyNotFoundException(
					q.Query
					+ " is not a valid assembly name (it should NOT end with "
					+ AssemblyQuery.ASSEMBLY_EXTENSION + ")"
				);

            Assembly a = null; 

			try
			{
                a = Assembly.Load(q.ToAssemblyName());
                return a;
            }
            catch (Exception err)
            {
                try 
                {
                    // try to load it from the current application directory
                    string assemblyPath = Path.Combine(Location.ApplicationDirectory, 
                                                       q.Query + AssemblyQuery.ASSEMBLY_EXTENSION);
                    a = Assembly.LoadFrom(assemblyPath);
                    return a;
                }
                catch(Exception e)
                {
                    #if COMPACT
                    throw new AssemblyNotFoundException(q.Query, e);
                    #else
                    // if all else failed, try LoadWithPartialName. This is 
                    // necessary for GTK# on Windows since it only provides 
                    // .NET 1.0 versions which are not found by Assembly.Load 
                    // on .NET 2.0.
                    a = Assembly.LoadWithPartialName(q.Query);
                    if (a != null)
                        return a;
                    else
                        throw new AssemblyNotFoundException(q.Query, e);
                    #endif
                }
            }
		}

		/// <summary>
		/// Tries to load an assembly in any possible way
		/// </summary>
		/// <param name="query">the query to look for (can be a partial name
		/// or a path to a file)</param>
		/// <returns>the loaded assembly</returns>
		public static Assembly LoadAny(string query)
		{
            return LoadAny(new AssemblyQuery(query));
		}

		private static Assembly LoadAny(AssemblyQuery q)
		{
			try
			{
				if (q.IsPath)
				{
					// try to load it from a path
					return LoadFromPath(q);
				}
				else // partial name
				{
					// try loading it from the GAC or current app dir
					return LoadFromGacOrAppDir(q);
				}
			}
			catch (AssemblyNotFoundException)
			{
				try
				{
					// try to convert it to the other format (as a last resort)
					if (q.IsPath)
					{
						// load from path failed
						return LoadFromGacOrAppDir(q.ToPartialName());
					}
					else
					{
						// load with partial name failed
						return LoadFromPath(q.ToPath());
					}
				}
				catch (AssemblyNotFoundException)
				{
					throw new AssemblyNotFoundException(
						string.Format(
						"The assembly {0} could neither be found in the GAC or at an absolute path",
						q.Query
						)
					);
				}
			}
		}
	}
}
