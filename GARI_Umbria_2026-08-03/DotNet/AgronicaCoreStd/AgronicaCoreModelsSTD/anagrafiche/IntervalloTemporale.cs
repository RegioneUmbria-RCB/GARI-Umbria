using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class IntervalloTemporale
    {
        // public int Id { get; set; }
        public DateTime inizio { get; set; }
        public DateTime fine { get; set; }

        public IntervalloTemporale(DateTime inizio, DateTime fine)
        {
            this.inizio = inizio;
            this.fine = fine;
        }

        public IntervalloTemporale() {
            this.inizio = new DateTime(1900, 1, 1);
            this.fine = new DateTime(2100, 12, 31);
        }

        /// <summary>
        /// Determines the duration of the interval in days.
        /// </summary>
        /// <returns>The number of days in the date interval.</returns>
        public int durataInGiorni()
        {
            return (int)(fine - inizio).TotalDays;
        }

        /// <summary>
        /// Determines if the interval is currently active (as of now).
        /// </summary>
        /// <returns>True if the interval is active, false otherwise.</returns>
        public bool isActive()
        {
            return isActive(DateTime.Now);
        }

        /// <summary>
        /// Determines if the interval is active at a specific reference date.
        /// </summary>
        /// <param name="referenceDate">The date to check against.</param>
        /// <returns>True if the interval contains the reference date, false otherwise.</returns>
        public bool isActive(DateTime referenceDate)
        {
            return this.contains(referenceDate);
        }

        /// <summary>
        /// Checks if this time interval overlaps with another time interval.
        /// </summary>
        /// <param name="timeInterval">The time interval to compare with.</param>
        /// <returns>True if the intervals overlap, false otherwise.</returns>
        public bool overlaps(IntervalloTemporale timeInterval)
        {
            DateTime start = this.inizio.Date;
            DateTime end = this.fine.Date;

            DateTime comparisonStart = timeInterval.inizio.Date;
            DateTime comparisonEnd = timeInterval.fine.Date;

            return end >= comparisonStart && start <= comparisonEnd;
        }

        /// <summary>
        /// Checks if the interval contains a specific date.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the interval contains the date, false otherwise.</returns>
        public bool contains(DateTime date)
        {
            DateTime start = this.inizio.Date;
            DateTime end = this.fine.Date;
            DateTime d = date.Date;

            return start <= d && end >= d;
        }
    }
}
