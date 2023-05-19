using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csElasticsearchClientIndexCreate
{
    public class DocumentationChunk
    {
        public string FileName { get; set; }
        public string ChunkNumber { get; set; }
        public string Extension { get; set; }
        public float[] EmbeddingVector { get; set; }
    }
}
