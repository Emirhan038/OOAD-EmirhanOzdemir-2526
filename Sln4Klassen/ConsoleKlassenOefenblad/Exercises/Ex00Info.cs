using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKlassenOefenblad.Exercises
{
    internal class Ex00Info
    {
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
=== Oefenblad: Klassen ===

Dit oefenblad bevat oefeningen over klassen in C#.
Elke oefening staat in een apart bestand onder 'Exercises/'.
De bijhorende klassen staan in 'Exercises/Classes/'.

Werkwijze:
1. Lees de uitleg commentaar bovenaan de oefening.
2. Maak de gevraagde klasse(s) aan in 'Exercises/Classes/'.
3. Schrijf waar gevraagd de nodige code
4. Haal waar gevraagd de testcode uit commentaar om je code te testen.
5. Controleer goed of de output overeenkomt met de meegegeven screenshot in Images/screenshot.png!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
