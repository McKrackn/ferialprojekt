using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace ProFer.Model
{
    public class Roll : ViewModelBase
    {
        public int Value;
        private BitmapImage _diceImage;
        public BitmapImage DiceImage
        {
            get => _diceImage;
            set => Set(ref _diceImage, value);
        }

        public Roll()
        {
            Value = new Random().Next(1,7);
            _diceImage = new BitmapImage(new Uri($"img\\{Value}.png", UriKind.Relative)); 
        }

        public Roll(int value)
        {
            Value = value;
            _diceImage = new BitmapImage(new Uri($"img\\{Value}.png", UriKind.Relative));
        }
    }
} 