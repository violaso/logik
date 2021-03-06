﻿using Cairo;
using LogikUI.Circuit;
using LogikUI.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LogikUI.Simulation.Gates
{
    class AndGate : IComponent
    {
        // Indices for the ports
        public string Name => "And Gate";
        public ComponentType Type => ComponentType.And;
        public int NumberOfPorts => 3;

        public void GetPorts(Span<Vector2i> ports)
        {
            ports[0] = new Vector2i(-3, 1);
            ports[1] = new Vector2i(-3, -1);
            ports[2] = new Vector2i(0, 0);
        }
        
        // FIXME: Cleanup and possibly split draw into a 'outline' and 'fill'
        // call so we can do more efficient cairo rendering.
        public void Draw(Context cr, InstanceData data)
        {
            using var transform = IComponent.ApplyComponentTransform(cr, data);

            //foreach (var gate in instances)
            {
                cr.MoveTo(-30,-15);
                cr.RelLineTo(15, 0);
                cr.RelCurveTo(20, 0, 20, 30, 0, 30);
                cr.RelLineTo(-15, 0);
                cr.ClosePath();
            }

            // FIXME: We probably shouldn't hardcode the color
            cr.SetSourceRGB(0.1, 0.1, 0.1);
            cr.LineWidth = Wires.WireWidth;
            cr.Stroke();

            //foreach (var gate in instances)
            {
                Span<Vector2i> points = stackalloc Vector2i[NumberOfPorts];
                GetPorts(points);

                foreach (var p in points) {
                    var port = p * CircuitEditor.DotSpacing;

                    // FIXME: Magic number radius...
                    cr.Arc(port.X, port.Y, 2, 0, Math.PI * 2);
                    cr.ClosePath();
                }
            }
            cr.SetSourceRGB(0.2, 0.9, 0.2);
            cr.Fill();
        }
    }
}
