using System;

namespace Client.MessageAttente
{
    public interface IController
    {
        bool Running { get; }
        bool Canceled { get; }
        void Display();
        void SetText(string text);
        void Failed(Exception e);
        void Completed(bool cancelled);
        /// <summary>
        /// Assigne l'état de visibilité du message d'attente
        /// </summary>
        /// <remarks></remarks>
        void SetVisible(bool visible);
    }
}
