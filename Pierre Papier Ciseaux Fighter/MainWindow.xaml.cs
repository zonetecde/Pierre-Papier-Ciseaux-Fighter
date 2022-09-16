using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pierre_Papier_Ciseaux_Fighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Taille du Width du Canvas. Note that width = height. - 20 est pour la taille d'un élément (pour ne pas qu'il sorte du canvas)
        private const int CANVAS_SIZE = 616;
        private double MOVE_SPEED = 1;

        // Timer sous lequel toutes l'animation fonctionnera
        Timer Timer;

        // Rdn
        Random Rdn;

        // Var game
        int NbCiseaux = 33;
        int NbPierre = 33;
        int NbPapier = 33;
        int ElementSize = 20;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Efface le contenu du Canvas puis relance un nouveau fight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Valider_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie que les informations rentrées dans les txtBoxs sont correctes, et si elles le sont les attribues à leur variable respectif
            if(!(String.IsNullOrEmpty(txtBox_ciseaux.Text) && String.IsNullOrEmpty(txtBox_papiers.Text) && String.IsNullOrEmpty(txtBox_pierres.Text))
                && int.TryParse(txtBox_ciseaux.Text, out NbCiseaux) && int.TryParse(txtBox_papiers.Text, out NbPapier) && int.TryParse(txtBox_pierres.Text, out NbPierre) )
            {
                // Reset l'ancien board
                Timer.Stop();
                Canvas_Board.Children.Clear();

                // Place les éléments dans le Canvas aléatoirement
                for (int i = 0; i < NbCiseaux; i++)
                    PlaceInCanvas(Rdn.Next(0, CANVAS_SIZE - ElementSize), Rdn.Next(0, CANVAS_SIZE - ElementSize), Canvas_Board, ELEMENT.CISEAUX);
                for (int i = 0; i < NbPapier; i++)
                    PlaceInCanvas(Rdn.Next(0, CANVAS_SIZE - ElementSize), Rdn.Next(0, CANVAS_SIZE - ElementSize), Canvas_Board, ELEMENT.PAPIER);
                for (int i = 0; i < NbPierre; i++)
                    PlaceInCanvas(Rdn.Next(0, CANVAS_SIZE - ElementSize), Rdn.Next(0, CANVAS_SIZE - ElementSize), Canvas_Board, ELEMENT.PIERRE);

                Timer.Start();
            }
        }

        /// <summary>
        /// Ajoute un élément dans le Canvas aux coordonnées données
        /// </summary>
        /// <param name="x">co</param>
        /// <param name="y">co</param>
        /// <param name="canva">Canvas où l'élément sera ajouté</param>
        private void PlaceInCanvas(int x, int y, Canvas canva, ELEMENT element)
        {
            // Init border (l'élément)
            Border border_element = new Border();

            // Taille
            border_element.Width = ElementSize;
            border_element.Height = ElementSize;

            // Tag
            border_element.Tag = element.ToString();

            // Bordure
            //border_element.BorderThickness = new Thickness(1);
            //border_element.BorderBrush = Brushes.Gray;

            // Image de l'élément
            if(element == ELEMENT.PIERRE)
                border_element.Background = resources_border_rock.Background;
            else if (element == ELEMENT.CISEAUX)
                border_element.Background = resources_border_scissors.Background;
            else
                border_element.Background = resources_border_paper.Background;
            

            // Ajoute l'élément dans le Canvas
            Canvas_Board.Children.Add(border_element);

            // Positionne l'élément dans le Canvas. Si on veut des positions aléatoires (checkBox_stackElement == true) on laisse
            // les co mis en paramètre, sinon les places dans un coin selon l'élément
            if (checkBox_stackElement.IsChecked == false)
            {
                Canvas.SetLeft(border_element, x);
                Canvas.SetTop(border_element, y);
            }
            else
            {
                if (element == ELEMENT.PIERRE) // Les pierres en haut au milieu du Canvas
                {
                    Canvas.SetLeft(border_element, CANVAS_SIZE / 2 - ElementSize / 2);
                    Canvas.SetTop(border_element, ElementSize + 20);
                }
                else if (element == ELEMENT.CISEAUX) // Les ciseaux en bas à gauche du Canvas
                {
                    Canvas.SetLeft(border_element, ElementSize + 20);
                    Canvas.SetTop(border_element, CANVAS_SIZE - ElementSize - 20);
                }
                else // Les papiers en bas à droite du Canvas
                {
                    Canvas.SetLeft(border_element, CANVAS_SIZE - ElementSize - 20);
                    Canvas.SetTop(border_element, CANVAS_SIZE - ElementSize - 20);
                }
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Init Timer
            Timer = new Timer(1);
            Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);

            // Init rdn
            Rdn = new Random();
        }

        /// <summary>
        /// Toutes les x secondes on fait bouger aléatoirement les éléments, et s'ils s'entrechoquent on convertit le perdant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // Fait bouger chaque élément aléatoirement de 1 pixel dans une des 8 directions
                foreach (Border border_element in Canvas_Board.Children)
                {
                    DIRECTION direction_aléatoire = (DIRECTION)Enum.GetValues(typeof(DIRECTION)).GetValue(Rdn.Next(Enum.GetValues(typeof(DIRECTION)).Length));
                    
                    if (direction_aléatoire == DIRECTION.HAUT)
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) - MOVE_SPEED);
                    else if (direction_aléatoire == DIRECTION.BAS)
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) + MOVE_SPEED);
                    else if (direction_aléatoire == DIRECTION.GAUCHE)
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) - MOVE_SPEED);
                    else if (direction_aléatoire == DIRECTION.DROITE)
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) + MOVE_SPEED);
                    else if (direction_aléatoire == DIRECTION.HAUT_GAUCHE)
                    {
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) - MOVE_SPEED);
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) - MOVE_SPEED);
                    }
                    else if (direction_aléatoire == DIRECTION.HAUT_DROIT)
                    {
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) - MOVE_SPEED);
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) + MOVE_SPEED);
                    }
                    else if (direction_aléatoire == DIRECTION.BAS_GAUCHE)
                    {
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) + MOVE_SPEED);
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) - MOVE_SPEED);
                    }
                    else if (direction_aléatoire == DIRECTION.BAS_DROIT)
                    {
                        Canvas.SetTop(border_element, Canvas.GetTop(border_element) + MOVE_SPEED);
                        Canvas.SetLeft(border_element, Canvas.GetLeft(border_element) + MOVE_SPEED);
                    }

                    // Vérifie qu'il n'est pas sortie hors des limites
                    if (Canvas.GetTop(border_element) < 0) // dessus
                        Canvas.SetTop(border_element, 1);
                    if (Canvas.GetLeft(border_element) < 0) // gauche
                        Canvas.SetLeft(border_element, 1);
                    if (Canvas.GetLeft(border_element) > CANVAS_SIZE - ElementSize) // droite
                        Canvas.SetLeft(border_element, CANVAS_SIZE - ElementSize - 1);
                    if (Canvas.GetTop(border_element) > CANVAS_SIZE - ElementSize) // bas
                        Canvas.SetTop(border_element, CANVAS_SIZE - ElementSize - 1);                    
                }


                // Check si deux éléments intercèdent entre eux
                for (int i = 0; i < Canvas_Board.Children.Count; i++)
                {
                    for (int j = i + 1; j < Canvas_Board.Children.Count; j++)
                    {
                        var intercession = Rect.Intersect(new Rect(Canvas.GetLeft(Canvas_Board.Children[i]), Canvas.GetTop(Canvas_Board.Children[i]), ElementSize, ElementSize),
                            new Rect(Canvas.GetLeft(Canvas_Board.Children[j]), Canvas.GetTop(Canvas_Board.Children[j]), ElementSize, ElementSize)) == Rect.Empty ? false : true;

                        // Si il y a intercession
                        if (intercession)
                        {
                            // Alors on regarde qui gagne entre les deux éléments
                            if (ChangeSiPerdant("PIERRE", "CISEAUX", i, j)) 
                                intercession = true;
                            else if (ChangeSiPerdant("PAPIER", "PIERRE", i, j)) 
                                intercession = true;
                            else ChangeSiPerdant("CISEAUX", "PAPIER", i, j);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Change l'élément en celui du gagnant 
        /// </summary>
        /// <param name="gagnantTag">Le tag gagnant</param>
        /// <param name="perdantTag">Le tag perdant</param>
        /// <param name="index_first">Index du gagnant ou du perdant</param>
        /// <param name="index_second">Index du gagnant ou du perdant</param>
        private bool ChangeSiPerdant(string gagnantTag, string perdantTag, int index_first, int index_second)
        {
            string firstTag = (Canvas_Board.Children[index_first] as Border).Tag.ToString();
            string secondTag = (Canvas_Board.Children[index_second] as Border).Tag.ToString();

            // Si l'un a gagné sur l'autre
            if ((firstTag == gagnantTag && perdantTag == secondTag)
                || (firstTag == perdantTag && gagnantTag == secondTag))
            {
                // Change le perdant en gagnant
                var toChange = firstTag == gagnantTag ? index_second : index_first;
                var toModel = toChange == index_second ? index_first : index_second;

                (Canvas_Board.Children[toChange] as Border).Background = (Canvas_Board.Children[toModel] as Border).Background;
                (Canvas_Board.Children[toChange] as Border).Tag = (Canvas_Board.Children[toModel] as Border).Tag;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Change l’intervalle du timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_FPS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Timer != null)
                Timer.Interval = e.NewValue;
        }

        /// <summary>
        /// Change le MOVE_SPEED (vitesse à laquelle se déplace les éléments)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_MoveSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MOVE_SPEED = e.NewValue;
        }

        /// <summary>
        /// Change la size des éléments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_SIZE_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ElementSize = Convert.ToInt32(e.NewValue);

            if(Canvas_Board != null)
            {
                foreach (Border b in Canvas_Board.Children)
                {
                    b.Width = ElementSize;
                    b.Height = ElementSize;
                }
            }
        }
    }

    enum ELEMENT
    {
        PIERRE,
        PAPIER,
        CISEAUX
    }

    enum DIRECTION
    {
        HAUT,
        DROITE,
        GAUCHE,
        BAS,
        HAUT_GAUCHE,
        HAUT_DROIT,
        BAS_GAUCHE,
        BAS_DROIT
    }
}
