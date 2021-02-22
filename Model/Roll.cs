using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace ProFer.Model
{
    public class Roll : ViewModelBase
    {
        public int Value;
        public string Description;
        private BitmapImage _diceImage;
        public BitmapImage DiceImage
        {
            get => _diceImage;
            set => Set(ref _diceImage, value);
        }
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                this.RaisePropertyChanged();
            }
        }
        public Roll()
        {
            Value = new Random().Next(1,7);
            _diceImage = new BitmapImage(new Uri($"img\\{Value}.png", UriKind.Relative));
            Description = Value switch
            {
                1 => "nine",
                2 => "ten",
                3 => "jack",
                4 => "queen",
                5 => "king",
                6 => "ace",
                _ => Description
            };
        }

        public Roll(int value)
        {
            Value = value;
            _diceImage = new BitmapImage(new Uri($"img\\{Value}.png", UriKind.Relative));
            Description = Value switch
            {
                1 => "nine",
                2 => "ten",
                3 => "jack",
                4 => "queen",
                5 => "king",
                6 => "ace",
                _ => Description
            };
        }

        public override string ToString()
        {
            return Description;
        }
    }
} 