using System.Collections.Generic;

namespace AlgorytmGenetyczny
{
    public class SimulationDto
    {
        public List<List<IndividualDto>> Populations { get; set; }
        public int Iterations { get; set; }
    }
}
