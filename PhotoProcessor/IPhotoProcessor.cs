using Galeriq.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galeriq.PhotoProcessor
{
    public interface IPhotoProcessor
    {
        Task ProcessPhotoAsync(PhotoProcessingMessage message);
    }
}
