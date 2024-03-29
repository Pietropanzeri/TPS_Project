﻿using GameClient.Controller;
using GameClient.Helpers;
using GameClient.Model;
using GameClient.Service;
using System.Collections.ObjectModel;
using WebSocketSharp;
using Cell = GameClient.Model.Cell;

namespace GameClient.model;

public class Bot : Utente
{
    private ImpostazioniController controller = ServiceHelper.GetService<ImpostazioniController>();
    private Random _random = new Random();

    public Cell Mossa(Game game)
    {
        while (true)
        {
            Cell cella = new Cell();
            if (cella.Content.IsNullOrEmpty())
            {
                Difficolta difficolta = controller.difficoltaBot;
                switch (difficolta)
                {
                    case Difficolta.Facile:
                        int pos = _random.Next(0, 9);
                        cella = game.GameField[pos];
                        break;
                    case Difficolta.Medio:
                        cella = game.GameField[TrovaMossaMedia(game)];
                        break;
                    case Difficolta.Impossibile:
                        cella = game.GameField[TrovaMossaImpossibile(game)];
                        break;
                }
            }

            //  cella = game.GameField[TrovaMossaMedia(game)];

            if (cella.Content.IsNullOrEmpty())
                return cella;
        }
    }
    static int TrovaMossaImpossibile(Game game)
    {
        string simboloBot = game.Players[1].Symbol;
        string simboloUtente = game.Players[0].Symbol;
        Collection<Cell> campo = game.GameField;
        int vuoto = 0;
        foreach (Cell cella in campo)
            if (cella.Content.IsNullOrEmpty())
                vuoto++;

        if (vuoto == 9)
        {
            //occupa centro
            if (campo[4].Content.IsNullOrEmpty())
                return 4;
        }

        //cerca la possibilità di vincere
        int? mossaVincente = MossaAttaccoODifesa(campo.ToList(), simboloBot);
        if (mossaVincente != null)
            return mossaVincente.Value;

        //cerca blocco
        int? mossaBloc = MossaAttaccoODifesa(campo.ToList(), simboloUtente);
        if (mossaBloc != null)
            return mossaBloc.Value;

        //mette al centro
        if (campo[4].Content.IsNullOrEmpty())
            return 4;

        //mette in un angolo
        bool angoliOpposti = false;
        if (vuoto == 6)
            angoliOpposti = ControllAngoli(campo, simboloUtente);
        if (!angoliOpposti)
        {
            int? angoloDisa = TrovaAngoloDisp(campo, vuoto);
            if (angoloDisa != null)
                return angoloDisa.Value;
        }

        //mette in un lato
        int? lato = TrovaMossaLato(campo);
        return lato.Value;

    }
    private static bool ControllAngoli(Collection<Cell> campo, string simboloUtente)
    {
        return (campo[0].Content == simboloUtente && campo[8].Content == simboloUtente) ||
           (campo[2].Content == simboloUtente && campo[6].Content == simboloUtente);
    }

    public static int? MossaAttaccoODifesa(List<Cell> campo, string segno)
    {
        #region Cerca orizzontalmente
        for (int i = 0; i < 9; i += 3)
        {
            string cellaCorrente = campo[i].Content;
            string cellaSucc = campo[i + 1].Content;
            string cellaSucc2 = campo[i + 2].Content;

            if (cellaCorrente == segno && cellaSucc == segno && cellaSucc2.IsNullOrEmpty())
                return i + 2;

            if (cellaCorrente == segno && cellaSucc.IsNullOrEmpty() && cellaSucc2 == segno)
                return i + 1;

            if (cellaCorrente.IsNullOrEmpty() && cellaSucc == segno && cellaSucc2 == segno)
                return i;
        }

        #endregion

        #region Cerca verticalmente
        for (int i = 0; i < 3; i++)
        {
            string cellaCorrente = campo[i].Content;
            string cellaSotto = campo[i + 3].Content;
            string cellaSotto2 = campo[i + 6].Content;

            if (cellaCorrente == segno && cellaSotto == segno && cellaSotto2.IsNullOrEmpty())
                return i + 6;
            if (cellaCorrente == segno && cellaSotto.IsNullOrEmpty() && cellaSotto2 == segno)
                return i + 3;
            if (cellaCorrente.IsNullOrEmpty() && cellaSotto == segno && cellaSotto2 == segno)
                return i;
        }
        #endregion

        #region Cerca diagonalmente
        // Cerca diagonalmente
        if (campo[0].Content == segno && campo[4].Content == segno && campo[8].Content.IsNullOrEmpty())
            return 8;
        if (campo[0].Content == segno && campo[4].Content.IsNullOrEmpty() && campo[8].Content == segno)
            return 4;
        if (campo[0].Content.IsNullOrEmpty() && campo[4].Content == segno && campo[8].Content == segno)
            return 0;

        if (campo[2].Content == segno && campo[4].Content == segno && campo[6].Content.IsNullOrEmpty())
            return 6;
        if (campo[2].Content == segno && campo[4].Content.IsNullOrEmpty() && campo[6].Content == segno)
            return 4;
        if (campo[2].Content.IsNullOrEmpty() && campo[4].Content == segno && campo[6].Content == segno)
            return 2;
        #endregion

        return null; // Nessuna mossa vincente trovata
    }
    private static int? TrovaAngoloDisp(Collection<Cell> campo, int vuoto)
    {
        int[] angoli = { 0, 2, 6, 8 };
        if (vuoto == 8)
        {
            Random r = new Random();
            return angoli[r.Next(0, 4)];
        }
        foreach (int angolo in angoli)
        {
            if (campo[angolo].Content.IsNullOrEmpty())
                return angolo;
        }
        return null; // Nessun angolo disponibile
    }
    private static int? TrovaMossaLato(Collection<Cell> campo)
    {
        int[] lati = { 1, 3, 5, 7 };
        foreach (int lato in lati)
        {
            if (campo[lato].Content.IsNullOrEmpty())
            {
                return lato;
            }
        }
        return null; // Nessun lato disponibile
    }

    static int TrovaMossaMedia(Game game) 
    {
        string simboloBot = game.Players[1].Symbol;
        string simboloUtente = game.Players[0].Symbol;
        Collection<Cell> campo = game.GameField;
        Random r=new Random();
        int vuoto = 0;
        int casella = 0;
        foreach (Cell cella in campo)
            if (cella.Content.IsNullOrEmpty())
                vuoto++;

        if (vuoto == 9)
        {
            casella =r.Next(0, 8);
            //occupa posizione random
            if (campo[casella].Content.IsNullOrEmpty())
                return casella;
        }

        //cerca la possibilità di vincere
        int? mossaVincente = MossaAttaccoODifesa(campo.ToList(), simboloBot);
        if (mossaVincente != null)
            return mossaVincente.Value;

        //cerca blocco
        int? mossaBloc = MossaAttaccoODifesa(campo.ToList(), simboloUtente);
        if (mossaBloc != null)
            return mossaBloc.Value;

        while (true)
        {
            casella = r.Next(0, 9);
            //occupa posizione random
            if (campo[casella].Content.IsNullOrEmpty())
                return casella;
           
        }
        
    }
}