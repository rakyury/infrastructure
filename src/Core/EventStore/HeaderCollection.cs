﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/* Copyright (c) 2012 Spark Software Ltd.
 * 
 * This source is subject to the GNU Lesser General Public License.
 * See: http://www.gnu.org/copyleft/lesser.html
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE. 
 */

namespace Spark.Infrastructure.EventStore
{
    /// <summary>
    /// A read-only collection of named values (headers).
    /// </summary>
    [Serializable]
    public sealed class HeaderCollection : ReadOnlyDictionary<String, Object>
    {
        /// <summary>
        /// Represents an empty <see cref="HeaderCollection"/>. This field is read-only.
        /// </summary>
        public static readonly HeaderCollection Empty = new HeaderCollection(new Dictionary<String, Object>());

        /// <summary>
        /// Initializes a new instance of <see cref="EventCollection"/>.
        /// </summary>
        /// <param name="dictionary">The set of named values used to populate this <see cref="HeaderCollection"/>.</param>
        public HeaderCollection(IDictionary<String, Object> dictionary)
            : base(dictionary)
        { }
    }
}