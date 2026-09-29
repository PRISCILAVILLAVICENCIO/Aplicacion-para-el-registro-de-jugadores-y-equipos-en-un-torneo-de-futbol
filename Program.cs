using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TorneoFutbolUEA
{
    // =========================================================================
    // UNIVERSIDAD ESTATAL AMAZÓNICA (UEA)
    // Asignatura: Estructura de Datos
    // Práctica #03: Implementación de Conjuntos (HashSet) y Mapas (Dictionary)
    // Caso de Estudio: Aplicación para el Registro de Jugadores y Equipos en un Torneo
    // Nivel: Principiante - Intermedio
    // =========================================================================

    #region MODELOS DE DATOS

    /// <summary>
    /// Representa a un jugador de fútbol registrado en el sistema.
    /// </summary>
    public class Jugador
    {
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public int Edad { get; set; }
        public string Posicion { get; set; }
        public int NumeroCamiseta { get; set; }

        public Jugador(string cedula, string nombreCompleto, int edad, string posicion, int numeroCamiseta)
        {
            Cedula = cedula;
            NombreCompleto = nombreCompleto;
            Edad = edad;
            Posicion = posicion;
            NumeroCamiseta = numeroCamiseta;
        }

        public override string ToString()
        {
            return string.Format("[C.I: {0}] {1} | Edad: {2} | Pos: {3} | Dorsal: #{4}",
                Cedula, NombreCompleto, Edad, Posicion, NumeroCamiseta);
        }
    }

    /// <summary>
    /// Representa un equipo de fútbol participante en el torneo.
    /// Utiliza un HashSet para almacenar las cédulas de sus jugadores de forma única.
    /// </summary>
    public class Equipo
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DirectorTecnico { get; set; }

        // CONJUNTO (HashSet): Almacena las cédulas de los jugadores inscritos en este equipo.
        // Garantiza que ningún jugador se inscriba dos veces en el mismo equipo y permite búsquedas O(1).
        public HashSet<string> JugadoresCedulas { get; set; }

        public Equipo(string codigo, string nombre, string directorTecnico)
        {
            Codigo = codigo.ToUpper();
            Nombre = nombre;
            DirectorTecnico = directorTecnico;
            JugadoresCedulas = new HashSet<string>();
        }

        public bool InscribirJugador(string cedula)
        {
            // Add devuelve true si el elemento no existía y fue agregado exitosamente
            return JugadoresCedulas.Add(cedula);
        }

        public bool RemoverJugador(string cedula)
        {
            // Remove devuelve true si el elemento existía en el conjunto y fue eliminado
            return JugadoresCedulas.Remove(cedula);
        }

        public bool ContieneJugador(string cedula)
        {
            // Contains en HashSet tiene complejidad temporal O(1) promedio
            return JugadoresCedulas.Contains(cedula);
        }
    }

    #endregion

    #region GESTOR DEL TORNEO (LÓGICA CON CONJUNTOS Y MAPAS)

    /// <summary>
    /// Clase administradora del torneo. Gestiona las estructuras de datos principales:
    /// - Mapas (Dictionary): para acceso directo por clave O(1) a Equipos y Jugadores.
    /// - Conjuntos (HashSet): para operaciones de teoría de conjuntos (Unión, Intersección, Diferencia).
    /// </summary>
    public class TorneoManager
    {
        // MAPA 1 (Dictionary): Código de Equipo -> Objeto Equipo
        private Dictionary<string, Equipo> _equipos;

        // MAPA 2 (Dictionary): Cédula del Jugador -> Objeto Jugador (Padrón Maestro)
        private Dictionary<string, Jugador> _registroGlobalJugadores;

        // CONJUNTO GLOBAL (HashSet): Cédulas de jugadores que tienen sanción disciplinaria
        private HashSet<string> _jugadoresSancionados;

        public TorneoManager()
        {
            _equipos = new Dictionary<string, Equipo>(StringComparer.OrdinalIgnoreCase);
            _registroGlobalJugadores = new Dictionary<string, Jugador>();
            _jugadoresSancionados = new HashSet<string>();

            // Cargar datos de demostración para facilidad de evaluación
            CargarDatosDePrueba();
        }

        #region MÉTODOS DE REGISTRO Y CONSULTA BÁSICA

        public bool RegistrarEquipo(string codigo, string nombre, string dt)
        {
            codigo = codigo.Trim().ToUpper();
            if (_equipos.ContainsKey(codigo))
            {
                return false; // Clave duplicada en el Mapa
            }

            Equipo nuevoEquipo = new Equipo(codigo, nombre, dt);
            _equipos.Add(codigo, nuevoEquipo);
            return true;
        }

        public bool RegistrarJugador(string cedula, string nombre, int edad, string posicion, int dorsal)
        {
            cedula = cedula.Trim();
            if (_registroGlobalJugadores.ContainsKey(cedula))
            {
                return false; // Ya existe en el padrón maestro
            }

            Jugador nuevoJugador = new Jugador(cedula, nombre, edad, posicion, dorsal);
            _registroGlobalJugadores.Add(cedula, nuevoJugador);
            return true;
        }

        public bool AsignarJugadorAEquipo(string codigoEquipo, string cedula)
        {
            codigoEquipo = codigoEquipo.Trim().ToUpper();
            cedula = cedula.Trim();

            // Verificamos que el equipo y el jugador existan en los mapas
            if (!_equipos.ContainsKey(codigoEquipo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Error: El equipo con código '{0}' no está registrado.", codigoEquipo);
                Console.ResetColor();
                return false;
            }

            if (!_registroGlobalJugadores.ContainsKey(cedula))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Error: El jugador con cédula '{0}' no está registrado en el padrón general.", cedula);
                Console.ResetColor();
                return false;
            }

            Equipo equipo = _equipos[codigoEquipo];
            bool agregado = equipo.InscribirJugador(cedula);

            if (!agregado)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" Aviso: El jugador ya se encuentra en el conjunto del equipo '{0}'.", equipo.Nombre);
                Console.ResetColor();
                return false;
            }

            return true;
        }

        public bool SancionarJugador(string cedula)
        {
            cedula = cedula.Trim();
            if (!_registroGlobalJugadores.ContainsKey(cedula))
            {
                return false;
            }
            return _jugadoresSancionados.Add(cedula);
        }

        public bool LevantarSancion(string cedula)
        {
            cedula = cedula.Trim();
            return _jugadoresSancionados.Remove(cedula);
        }

        public Jugador ObtenerJugadorPorCedula(string cedula)
        {
            cedula = cedula.Trim();
            Jugador jugador;
            if (_registroGlobalJugadores.TryGetValue(cedula, out jugador))
            {
                return jugador;
            }
            return null;
        }

        public Equipo ObtenerEquipoPorCodigo(string codigo)
        {
            codigo = codigo.Trim().ToUpper();
            Equipo equipo;
            if (_equipos.TryGetValue(codigo, out equipo))
            {
                return equipo;
            }
            return null;
        }

        #endregion

        #region OPERACIONES DE TEORÍA DE CONJUNTOS (SET THEORY)

        /// <summary>
        /// UNIÓN (A ∪ B): Combina los jugadores de dos equipos en un solo conjunto resultante sin duplicados.
        /// </summary>
        public HashSet<string> OperacionUnion(string codEquipoA, string codEquipoB)
        {
            Equipo eqA = ObtenerEquipoPorCodigo(codEquipoA);
            Equipo eqB = ObtenerEquipoPorCodigo(codEquipoB);

            if (eqA == null || eqB == null) return null;

            // Clonamos el conjunto de A para no alterar los datos originales
            HashSet<string> resultadoUnion = new HashSet<string>(eqA.JugadoresCedulas);
            // Aplicamos la operación matemática de Unión
            resultadoUnion.UnionWith(eqB.JugadoresCedulas);

            return resultadoUnion;
        }

        /// <summary>
        /// INTERSECCIÓN (A ∩ B): Identifica los jugadores que están presentes simultáneamente en ambos equipos.
        /// (Útil para detectar duplicidades o inscripciones irregulares).
        /// </summary>
        public HashSet<string> OperacionInterseccion(string codEquipoA, string codEquipoB)
        {
            Equipo eqA = ObtenerEquipoPorCodigo(codEquipoA);
            Equipo eqB = ObtenerEquipoPorCodigo(codEquipoB);

            if (eqA == null || eqB == null) return null;

            HashSet<string> resultadoInterseccion = new HashSet<string>(eqA.JugadoresCedulas);
            // Aplicamos la operación matemática de Intersección
            resultadoInterseccion.IntersectWith(eqB.JugadoresCedulas);

            return resultadoInterseccion;
        }

        /// <summary>
        /// DIFERENCIA (A \ Sancionados): Determina los jugadores del equipo que están legalmente HABILITADOS
        /// para jugar el partido, restando el conjunto de sancionados.
        /// </summary>
        public HashSet<string> OperacionDiferenciaHabilitados(string codEquipo)
        {
            Equipo eq = ObtenerEquipoPorCodigo(codEquipo);
            if (eq == null) return null;

            HashSet<string> resultadoHabilitados = new HashSet<string>(eq.JugadoresCedulas);
            // Aplicamos la operación matemática de Diferencia (ExceptWith)
            resultadoHabilitados.ExceptWith(_jugadoresSancionados);

            return resultadoHabilitados;
        }

        #endregion

        #region REPORTERÍA Y VISUALIZACIÓN

        public void MostrarReporteGeneral()
        {
            Program.LimpiarPantalla();
            ImprimirEncabezado("REPORTE GENERAL DEL TORNEO");

            Console.WriteLine(" Total de Equipos Registrados   : {0}", _equipos.Count);
            Console.WriteLine(" Total de Jugadores en Padrón   : {0}", _registroGlobalJugadores.Count);
            Console.WriteLine(" Total de Jugadores Sancionados : {0}", _jugadoresSancionados.Count);
            Console.WriteLine(new string('-', 75));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0,-8} | {1,-25} | {2,-20} | {3,-10}", "CÓDIGO", "NOMBRE DEL EQUIPO", "DIRECTOR TÉCNICO", "JUGADORES");
            Console.WriteLine(new string('-', 75));
            Console.ResetColor();

            foreach (var kvp in _equipos)
            {
                Equipo eq = kvp.Value;
                Console.WriteLine("{0,-8} | {1,-25} | {2,-20} | {3,-10}",
                    eq.Codigo, eq.Nombre, eq.DirectorTecnico, eq.JugadoresCedulas.Count);
            }
            Console.WriteLine(new string('-', 75));
        }

        public void MostrarPlantillaEquipo(string codigoEquipo)
        {
            Equipo eq = ObtenerEquipoPorCodigo(codigoEquipo);
            if (eq == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Equipo no encontrado.");
                Console.ResetColor();
                return;
            }

            ImprimirEncabezado("PLANTILLA OFICIAL: " + eq.Nombre + " (" + eq.Codigo + ")");
            Console.WriteLine(" Director Técnico: {0}", eq.DirectorTecnico);
            Console.WriteLine(" Total de Jugadores en el Conjunto: {0}", eq.JugadoresCedulas.Count);
            Console.WriteLine(new string('-', 85));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8} | {5,-10}",
                "CÉDULA", "NOMBRE COMPLETO", "EDAD", "POSICIÓN", "DORSAL", "ESTADO");
            Console.WriteLine(new string('-', 85));
            Console.ResetColor();

            foreach (string cedula in eq.JugadoresCedulas)
            {
                Jugador j = ObtenerJugadorPorCedula(cedula);
                bool estaSancionado = _jugadoresSancionados.Contains(cedula);
                string estadoTexto = estaSancionado ? "SANCIONADO" : "HABILITADO";

                if (estaSancionado) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Green;

                if (j != null)
                {
                    Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8} | {5,-10}",
                        j.Cedula, j.NombreCompleto, j.Edad, j.Posicion, "#" + j.NumeroCamiseta, estadoTexto);
                }
                else
                {
                    Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8} | {5,-10}",
                        cedula, "No registrado en padrón", "-", "-", "-", estadoTexto);
                }
                Console.ResetColor();
            }
            Console.WriteLine(new string('-', 85));
        }

        public void MostrarReporteSancionados()
        {
            ImprimirEncabezado("LISTA DE JUGADORES SANCIONADOS (CONJUNTO DE SANCIONES)");
            if (_jugadoresSancionados.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" No hay jugadores sancionados actualmente. Fair Play!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0,-12} | {1,-25} | {2,-20} | {3,-10}", "CÉDULA", "NOMBRE", "EQUIPO(S)", "DORSAL");
            Console.WriteLine(new string('-', 75));
            Console.ResetColor();

            foreach (string cedula in _jugadoresSancionados)
            {
                Jugador j = ObtenerJugadorPorCedula(cedula);
                string nombre = j != null ? j.NombreCompleto : "Desconocido";
                string dorsal = j != null ? "#" + j.NumeroCamiseta : "-";

                // Buscar en qué equipo(s) está jugando
                List<string> equiposDelJugador = new List<string>();
                foreach (var eq in _equipos.Values)
                {
                    if (eq.ContieneJugador(cedula))
                    {
                        equiposDelJugador.Add(eq.Nombre);
                    }
                }
                string equiposStr = equiposDelJugador.Count > 0 ? string.Join(", ", equiposDelJugador.ToArray()) : "Sin equipo";

                Console.WriteLine("{0,-12} | {1,-25} | {2,-20} | {3,-10}", cedula, nombre, equiposStr, dorsal);
            }
            Console.WriteLine(new string('-', 75));
        }

        public void MostrarResultadoConjunto(string titulo, HashSet<string> conjuntoCedulas)
        {
            ImprimirEncabezado(titulo);
            if (conjuntoCedulas == null || conjuntoCedulas.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" El conjunto resultante está vacío (0 elementos).");
                Console.ResetColor();
                return;
            }

            Console.WriteLine(" Total de elementos en el conjunto: {0}", conjuntoCedulas.Count);
            Console.WriteLine(new string('-', 75));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8}",
                "CÉDULA", "NOMBRE", "EDAD", "POSICIÓN", "DORSAL");
            Console.WriteLine(new string('-', 75));
            Console.ResetColor();

            foreach (string ced in conjuntoCedulas)
            {
                Jugador j = ObtenerJugadorPorCedula(ced);
                if (j != null)
                {
                    Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8}",
                        j.Cedula, j.NombreCompleto, j.Edad, j.Posicion, "#" + j.NumeroCamiseta);
                }
                else
                {
                    Console.WriteLine("{0,-12} | {1,-25} | {2,-6} | {3,-15} | {4,-8}",
                        ced, "(Sin datos en padrón)", "-", "-", "-");
                }
            }
            Console.WriteLine(new string('-', 75));
        }

        #endregion

        #region BENCHMARKING Y ANÁLISIS DE TIEMPOS DE EJECUCIÓN (STOPWATCH)

        /// <summary>
        /// Realiza pruebas de rendimiento con Stopwatch comparando:
        /// 1. Búsqueda en HashSet (Conjunto) vs Búsqueda en List (Lista Lineal)
        /// 2. Búsqueda en Dictionary (Mapa) vs Búsqueda en List (Lista Lineal)
        /// </summary>
        public void EjecutarPruebaDeRendimiento()
        {
            Program.LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("BENCHMARK: ANÁLISIS DE TIEMPO DE EJECUCIÓN (CONJUNTOS Y MAPAS VS LISTAS)");

            int cantidadElementos = 100000; // 100,000 registros para una prueba contundente
            Console.WriteLine(" Configurando entorno de prueba con {0:N0} elementos sintéticos...", cantidadElementos);

            HashSet<string> hashSetTest = new HashSet<string>();
            Dictionary<string, string> dictTest = new Dictionary<string, string>();
            List<string> listTest = new List<string>();

            // Poblar estructuras
            for (int i = 0; i < cantidadElementos; i++)
            {
                string key = "CED-" + i.ToString("D7");
                hashSetTest.Add(key);
                dictTest.Add(key, "Jugador " + i);
                listTest.Add(key);
            }

            // Clave a buscar (la última para forzar el peor caso en búsqueda lineal O(N))
            string claveBuscar = "CED-" + (cantidadElementos - 1).ToString("D7");
            string claveInexistente = "CED-9999999";

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("\n[1] Prueba de Búsqueda Existente (Peor caso para List): '{0}'", claveBuscar);
            Console.WriteLine(new string('-', 75));

            // Test List<T>
            sw.Restart();
            bool halladoList = listTest.Contains(claveBuscar);
            sw.Stop();
            long tiempoListTicks = sw.ElapsedTicks;
            double tiempoListMs = sw.Elapsed.TotalMilliseconds;

            // Test HashSet<T>
            sw.Restart();
            bool halladoHash = hashSetTest.Contains(claveBuscar);
            sw.Stop();
            long tiempoHashTicks = sw.ElapsedTicks;
            double tiempoHashMs = sw.Elapsed.TotalMilliseconds;

            // Test Dictionary<TKey, TValue>
            sw.Restart();
            bool halladoDict = dictTest.ContainsKey(claveBuscar);
            sw.Stop();
            long tiempoDictTicks = sw.ElapsedTicks;
            double tiempoDictMs = sw.Elapsed.TotalMilliseconds;

            Console.WriteLine(" - List<string>.Contains       [O(N)] : {0,8:F4} ms ({1,6} ticks)", tiempoListMs, tiempoListTicks);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" - HashSet<string>.Contains    [O(1)] : {0,8:F4} ms ({1,6} ticks)", tiempoHashMs, tiempoHashTicks);
            Console.WriteLine(" - Dictionary<K,V>.ContainsKey [O(1)] : {0,8:F4} ms ({1,6} ticks)", tiempoDictMs, tiempoDictTicks);
            Console.ResetColor();

            Console.WriteLine("\n[2] Prueba de Búsqueda de Elemento Inexistente: '{0}'", claveInexistente);
            Console.WriteLine(new string('-', 75));

            sw.Restart();
            listTest.Contains(claveInexistente);
            sw.Stop();
            long tiempoListNo = sw.ElapsedTicks;

            sw.Restart();
            hashSetTest.Contains(claveInexistente);
            sw.Stop();
            long tiempoHashNo = sw.ElapsedTicks;

            sw.Restart();
            dictTest.ContainsKey(claveInexistente);
            sw.Stop();
            long tiempoDictNo = sw.ElapsedTicks;

            Console.WriteLine(" - List<string> (recorre todo) [O(N)] : {0,6} ticks", tiempoListNo);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" - HashSet<string> (función hash) [O(1)] : {0,6} ticks", tiempoHashNo);
            Console.WriteLine(" - Dictionary<K,V> (tabla hash)   [O(1)] : {0,6} ticks", tiempoDictNo);
            Console.ResetColor();

            Console.WriteLine("\n======================= CONCLUSIÓN TEÓRICA =======================");
            Console.WriteLine(" • List<T> requiere comparar elemento por elemento (O(N)).");
            Console.WriteLine(" • HashSet<T> y Dictionary<K,V> calculan la posición mediante");
            Console.WriteLine("   GetHashCode() de inmediato (O(1) tiempo constante).");
            Console.WriteLine(" • En colecciones grandes, Conjuntos y Mapas son cientos de veces");
            Console.WriteLine("   más rápidos para búsquedas, inserciones y validaciones.");
            Console.WriteLine("==================================================================");
        }

        #endregion

        #region DATOS DE PRUEBA (MOCK DATA)

        private void CargarDatosDePrueba()
        {
            // Registrar Equipos de la Amazonía Ecuatoriana
            RegistrarEquipo("EQ01", "Amazonas F.C.", "Carlos 'Pibe' Valderrama");
            RegistrarEquipo("EQ02", "Puyo Sporting", "Alex Aguinaga");
            RegistrarEquipo("EQ03", "Napo United", "Jorge Célico");

            // Registrar Padrón de Jugadores
            RegistrarJugador("1600112233", "Javier 'Kitu' Mina", 24, "Delantero", 9);
            RegistrarJugador("1600445566", "Mateo Chimbo", 21, "Mediocampista", 8);
            RegistrarJugador("1600778899", "Bryan Grefa", 27, "Defensa", 4);
            RegistrarJugador("1600990011", "Esteban Vargas", 29, "Arquero", 1);
            RegistrarJugador("1600223344", "Luis Shiguango", 22, "Extremo", 11);
            RegistrarJugador("1600556677", "David Andy", 25, "Volante de marca", 5);
            RegistrarJugador("1600889900", "Carlos Tanguila", 23, "Lateral Derecho", 2);
            RegistrarJugador("1600334455", "Christian Coquinche", 28, "Delantero Centro", 10);
            RegistrarJugador("1600667788", "Franklin Cerda", 20, "Defensa Central", 3);
            RegistrarJugador("1600123456", "Edison Mamallacta", 26, "Mediocampista", 7);

            // Asignar a Plantillas
            // Amazonas F.C.
            AsignarJugadorAEquipo("EQ01", "1600112233");
            AsignarJugadorAEquipo("EQ01", "1600445566");
            AsignarJugadorAEquipo("EQ01", "1600778899");
            AsignarJugadorAEquipo("EQ01", "1600990011");

            // Puyo Sporting
            AsignarJugadorAEquipo("EQ02", "1600223344");
            AsignarJugadorAEquipo("EQ02", "1600556677");
            AsignarJugadorAEquipo("EQ02", "1600889900");
            AsignarJugadorAEquipo("EQ02", "1600445566"); // Jugador compartido / doble inscripción para probar Intersección

            // Napo United
            AsignarJugadorAEquipo("EQ03", "1600334455");
            AsignarJugadorAEquipo("EQ03", "1600667788");
            AsignarJugadorAEquipo("EQ03", "1600123456");

            // Sancionar a un jugador para probar la Diferencia de Conjuntos
            SancionarJugador("1600778899"); // Bryan Grefa está sancionado
        }

        #endregion

        #region UTILIDADES DE CONSOLA

        public static void ImprimirEncabezado(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===============================================================================");
            Console.WriteLine("   " + titulo);
            Console.WriteLine("===============================================================================");
            Console.ResetColor();
        }

        #endregion
    }

    #endregion

    #region PROGRAMA PRINCIPAL Y MENÚ INTERACTIVO

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch { }

            try
            {
                Console.Title = "UEA - Práctica 03: Conjuntos y Mapas en C# (Torneo de Fútbol)";
            }
            catch { }

            TorneoManager gestor = new TorneoManager();

            bool continuar = true;

            while (continuar)
            {
                LimpiarPantalla();
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine();
                if (opcion == null) break;

                switch (opcion.Trim())
                {
                    case "1":
                        MenuRegistrarEquipo(gestor);
                        break;
                    case "2":
                        MenuRegistrarJugador(gestor);
                        break;
                    case "3":
                        MenuAsignarJugador(gestor);
                        break;
                    case "4":
                        MenuGestionarSanciones(gestor);
                        break;
                    case "5":
                        MenuOperacionesConjuntos(gestor);
                        break;
                    case "6":
                        MenuReporteria(gestor);
                        break;
                    case "7":
                        gestor.EjecutarPruebaDeRendimiento();
                        Pausar();
                        break;
                    case "8":
                        MenuAcercaDe();
                        Pausar();
                        break;
                    case "0":
                        continuar = false;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n Gracias por utilizar el Sistema de Gestión del Torneo UEA. ¡Éxitos!");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n Opción no válida. Presione Enter para reintentar...");
                        Console.ResetColor();
                        Pausar();
                        break;
                }
            }
        }

        static void MostrarMenuPrincipal()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===============================================================================");
            Console.WriteLine("        UNIVERSIDAD ESTATAL AMAZÓNICA - ESTRUCTURA DE DATOS (UNIDAD III)       ");
            Console.WriteLine("           SISTEMA DE GESTIÓN DE TORNEO DE FÚTBOL CON CONJUNTOS Y MAPAS        ");
            Console.WriteLine("===============================================================================");
            Console.ResetColor();

            Console.WriteLine("  [1] Registrar Nuevo Equipo (Mapa: Dictionary<string, Equipo>)");
            Console.WriteLine("  [2] Registrar Nuevo Jugador en Padrón (Mapa: Dictionary<string, Jugador>)");
            Console.WriteLine("  [3] Inscribir Jugador a Plantilla de Equipo (Conjunto: HashSet<string>)");
            Console.WriteLine("  [4] Gestión de Sanciones Disciplinarias (Conjunto: HashSet<string>)");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  [5] Operaciones de Teoría de Conjuntos (Unión, Intersección, Diferencia)");
            Console.ResetColor();
            Console.WriteLine("  [6] Módulo de Reportería y Consultas");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  [7] Medición de Rendimiento y Benchmark de Tiempos (Stopwatch O(1) vs O(N))");
            Console.ResetColor();
            Console.WriteLine("  [8] Información del Proyecto y Agente de IA Utilizado");
            Console.WriteLine("  [0] Salir del Sistema");
            Console.WriteLine("===============================================================================");
            Console.Write(" Seleccione una opción [0-8]: ");
        }

        static void MenuRegistrarEquipo(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("REGISTRAR NUEVO EQUIPO (MAPA)");

            Console.Write(" Ingrese código único del equipo (ej. EQ04): ");
            string codigo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(codigo)) { Error("El código no puede estar vacío."); return; }

            Console.Write(" Ingrese nombre del equipo: ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombre)) { Error("El nombre no puede estar vacío."); return; }

            Console.Write(" Ingrese nombre del Director Técnico: ");
            string dt = Console.ReadLine();

            if (gestor.RegistrarEquipo(codigo, nombre, dt))
            {
                Exito(string.Format("Equipo '{0}' registrado exitosamente en el Mapa.", nombre));
            }
            else
            {
                Error("Ya existe un equipo con ese código en el sistema.");
            }
            Pausar();
        }

        static void MenuRegistrarJugador(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("REGISTRAR NUEVO JUGADOR EN EL PADRÓN (MAPA GLOBAL)");

            Console.Write(" Ingrese número de cédula / DNI: ");
            string cedula = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(cedula)) { Error("La cédula no puede estar vacía."); return; }

            Console.Write(" Ingrese nombre completo del jugador: ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombre)) { Error("El nombre no puede estar vacío."); return; }

            Console.Write(" Ingrese edad: ");
            int edad;
            if (!int.TryParse(Console.ReadLine(), out edad) || edad < 15 || edad > 60)
            {
                Error("Edad inválida (debe ser un número entre 15 y 60 años).");
                return;
            }

            Console.Write(" Ingrese posición (ej. Arquero, Defensa, Mediocampista, Delantero): ");
            string posicion = Console.ReadLine();

            Console.Write(" Ingrese número de camiseta (1-99): ");
            int dorsal;
            if (!int.TryParse(Console.ReadLine(), out dorsal) || dorsal < 1 || dorsal > 99)
            {
                Error("Dorsal inválido (debe ser un número entre 1 y 99).");
                return;
            }

            if (gestor.RegistrarJugador(cedula, nombre, edad, posicion, dorsal))
            {
                Exito(string.Format("Jugador '{0}' guardado en el Mapa maestro de jugadores.", nombre));
            }
            else
            {
                Error("Ya existe un jugador registrado con esa cédula.");
            }
            Pausar();
        }

        static void MenuAsignarJugador(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("INSCRIBIR JUGADOR A PLANTILLA (CONJUNTO DE EQUIPO)");

            Console.Write(" Ingrese el código del equipo destino (ej. EQ01): ");
            string codEquipo = Console.ReadLine();

            Console.Write(" Ingrese la cédula del jugador a inscribir: ");
            string cedula = Console.ReadLine();

            if (gestor.AsignarJugadorAEquipo(codEquipo, cedula))
            {
                Exito("Jugador añadido con éxito al conjunto de la plantilla.");
            }
            Pausar();
        }

        static void MenuGestionarSanciones(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("GESTIÓN DE SANCIONES DISCIPLINARIAS (CONJUNTO SANCIONADOS)");

            Console.WriteLine(" [1] Sancionar a un Jugador (Agregar a HashSet)");
            Console.WriteLine(" [2] Levantar Sanción / Habilitar Jugador (Eliminar de HashSet)");
            Console.WriteLine(" [3] Ver Lista de Sancionados");
            Console.Write(" Seleccione opción [1-3]: ");
            string op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write(" Ingrese cédula del jugador a sancionar: ");
                string ced = Console.ReadLine();
                if (gestor.SancionarJugador(ced))
                {
                    Exito("Jugador agregado al conjunto de sancionados.");
                }
                else
                {
                    Error("No se pudo sancionar (compruebe si la cédula existe o si ya estaba sancionado).");
                }
            }
            else if (op == "2")
            {
                Console.Write(" Ingrese cédula del jugador para levantar sanción: ");
                string ced = Console.ReadLine();
                if (gestor.LevantarSancion(ced))
                {
                    Exito("Sanción levantada. El jugador ha sido removido del conjunto de sancionados.");
                }
                else
                {
                    Error("El jugador no se encontraba en el conjunto de sancionados.");
                }
            }
            else if (op == "3")
            {
                gestor.MostrarReporteSancionados();
            }
            Pausar();
        }

        static void MenuOperacionesConjuntos(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("OPERACIONES DE TEORÍA DE CONJUNTOS (SET THEORY)");

            Console.WriteLine(" [1] UNIÓN (A ∪ B)               -> Todos los jugadores de dos equipos combinados");
            Console.WriteLine(" [2] INTERSECCIÓN (A ∩ B)        -> Jugadores presentes en AMBOS equipos (doble ficha)");
            Console.WriteLine(" [3] DIFERENCIA (Equipo \\ Sanc)   -> Jugadores del equipo legalmente HABILITADOS");
            Console.Write(" Seleccione la operación a realizar [1-3]: ");
            string op = Console.ReadLine();

            if (op == "1" || op == "2")
            {
                Console.Write("\n Ingrese el código del Primer Equipo (ej. EQ01): ");
                string eqA = Console.ReadLine();
                Console.Write(" Ingrese el código del Segundo Equipo (ej. EQ02): ");
                string eqB = Console.ReadLine();

                if (op == "1")
                {
                    HashSet<string> res = gestor.OperacionUnion(eqA, eqB);
                    if (res != null)
                        gestor.MostrarResultadoConjunto("RESULTADO DE LA UNIÓN: " + eqA.ToUpper() + " ∪ " + eqB.ToUpper(), res);
                    else
                        Error("Uno o ambos códigos de equipo no existen.");
                }
                else
                {
                    HashSet<string> res = gestor.OperacionInterseccion(eqA, eqB);
                    if (res != null)
                        gestor.MostrarResultadoConjunto("RESULTADO DE LA INTERSECCIÓN: " + eqA.ToUpper() + " ∩ " + eqB.ToUpper(), res);
                    else
                        Error("Uno o ambos códigos de equipo no existen.");
                }
            }
            else if (op == "3")
            {
                Console.Write("\n Ingrese el código del Equipo a consultar habilitados: ");
                string eq = Console.ReadLine();
                HashSet<string> res = gestor.OperacionDiferenciaHabilitados(eq);
                if (res != null)
                    gestor.MostrarResultadoConjunto("JUGADORES HABILITADOS (Plantilla \\ Sancionados) DE " + eq.ToUpper(), res);
                else
                    Error("El código de equipo no existe.");
            }
            Pausar();
        }

        static void MenuReporteria(TorneoManager gestor)
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("MÓDULO DE REPORTERÍA Y CONSULTAS");

            Console.WriteLine(" [1] Reporte General del Torneo (Equipos y Estadísticas)");
            Console.WriteLine(" [2] Consultar Plantilla Específica de un Equipo");
            Console.WriteLine(" [3] Consultar Ficha de Jugador por Cédula (Búsqueda Directa O(1))");
            Console.WriteLine(" [4] Ver Lista de Jugadores Sancionados");
            Console.Write(" Seleccione opción [1-4]: ");
            string op = Console.ReadLine();

            if (op == "1")
            {
                gestor.MostrarReporteGeneral();
            }
            else if (op == "2")
            {
                Console.Write(" Ingrese el código del equipo (ej. EQ01): ");
                string cod = Console.ReadLine();
                gestor.MostrarPlantillaEquipo(cod);
            }
            else if (op == "3")
            {
                Console.Write(" Ingrese la cédula del jugador a buscar: ");
                string ced = Console.ReadLine();
                Jugador j = gestor.ObtenerJugadorPorCedula(ced);
                if (j != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n >>> DATOS DEL JUGADOR ENCONTRADO EN MAPA: <<<");
                    Console.WriteLine(" Cédula         : {0}", j.Cedula);
                    Console.WriteLine(" Nombre Completo: {0}", j.NombreCompleto);
                    Console.WriteLine(" Edad           : {0} años", j.Edad);
                    Console.WriteLine(" Posición       : {0}", j.Posicion);
                    Console.WriteLine(" Camiseta       : #{0}", j.NumeroCamiseta);
                    Console.ResetColor();
                }
                else
                {
                    Error("Jugador no encontrado en el mapa general de datos.");
                }
            }
            else if (op == "4")
            {
                gestor.MostrarReporteSancionados();
            }
            Pausar();
        }

        static void MenuAcercaDe()
        {
            LimpiarPantalla();
            TorneoManager.ImprimirEncabezado("INFORMACIÓN ACADÉMICA Y AGENTE DE IA");
            Console.WriteLine(" • Institución : Universidad Estatal Amazónica (UEA)");
            Console.WriteLine(" • Asignatura  : Estructura de Datos (Unidad III: Conjuntos y Mapas)");
            Console.WriteLine(" • Práctica    : #03 - Implementación de Conjuntos y Mapas");
            Console.WriteLine(" • Caso        : Registro de Jugadores y Equipos en Torneo de Fútbol");
            Console.WriteLine(" --------------------------------------------------------------------------");
            Console.WriteLine(" • Agente de IA Utilizado : Antigravity AI (Google DeepMind - Gemini 3.7)");
            Console.WriteLine(" • Rol del Agente         : Asistente de Programación y Arquitectura");
            Console.WriteLine(" • Porcentaje de Código   : ~60% Asistencia del Agente / 40% Estructuración y Lógica Humana");
            Console.WriteLine(" • Estructuras Aplicadas  : HashSet<T> (Conjuntos), Dictionary<K,V> (Mapas/Diccionarios)");
            Console.WriteLine(" • Herramienta Medición   : System.Diagnostics.Stopwatch");
            Console.WriteLine(" ==========================================================================");
        }

        #endregion

        #region MÉTODOS AUXILIARES DE INTERFAZ

        public static void LimpiarPantalla()
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                // Si la consola está redirigida, simplemente imprimimos saltos de línea
                Console.WriteLine("\n-------------------------------------------------------------------------------");
            }
        }

        static void Exito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n [ÉXITO] " + mensaje);
            Console.ResetColor();
        }

        static void Error(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n [ERROR] " + mensaje);
            Console.ResetColor();
        }

        static void Pausar()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n Presione Enter para continuar...");
            Console.ResetColor();
            try
            {
                Console.ReadLine();
            }
            catch
            {
                // Ignorar si el flujo ya se cerró
            }
        }

        #endregion
    }
}
