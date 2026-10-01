# Práctica #03: Conjuntos y Mapas en C# - UEA

Este repositorio contiene la solución completa y funcional para la **Guía de Prácticas #03: Implementación de conjuntos y mapas** de la asignatura **Estructura de Datos** de la **Universidad Estatal Amazónica (UEA)**.

---

## 📌 Proyecto Seleccionado
**Aplicación para el registro de jugadores y equipos en un torneo de fútbol** utilizando:
- **Mapas (`Dictionary<string, T>`)**: Registro de equipos por código y padrón maestro de jugadores por cédula ($O(1)$).
- **Conjuntos (`HashSet<string>`)**: Nóminas únicas de jugadores por equipo y conjunto de jugadores sancionados.
- **Teoría de Conjuntos**: Unión ($A \cup B$), Intersección ($A \cap B$), Diferencia ($A \setminus Sancionados$).
- **Reportería**: Visualización de plantillas, listas de sancionados y consultas directas.
- **Medición de Rendimiento (`Stopwatch`)**: Módulo de benchmark que demuestra empíricamente la ventaja de $O(1)$ frente a listas lineales $O(N)$.

---

## 📁 Estructura del Directorio

```text
d:\WORK 2026\Personal\practica1\
│
├── Program.cs             # Código fuente principal en C# (Nivel principiante-intermedio)
├── INFORME_PRACTICA_03.md # Informe académico completo formato UEA y normas APA 7ma Edición
├── compilar.bat           # Script para compilar con el compilador nativo de Windows (csc.exe)
├── ejecutar.bat           # Script para compilar y ejecutar con un solo clic
└── README.md              # Documentación del proyecto
```

---

## 🚀 Cómo Compilar y Ejecutar

### Opción 1: Ejecución con 1 clic (Recomendada)
Haz doble clic sobre el archivo **`ejecutar.bat`**.



## 🤖 Agente de Inteligencia Artificial Utilizado
- **Agente:** Antigravity AI (Google DeepMind / Modelo Gemini 3.7)
- **Rol:** Asistente de arquitectura, estructura modular y algoritmos de teoría de conjuntos y benchmark.
- **Porcentaje de Código:** ~60% Asistencia de IA / 40% Parametrización y Lógica Humana.
