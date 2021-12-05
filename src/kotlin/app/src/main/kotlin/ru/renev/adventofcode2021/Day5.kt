package ru.renev.adventofcode2021

class Day5 {
    data class Coords(val x: Int, val y: Int) {
        override fun toString(): String {
            return "x=$x,y=$y"
        }
    }

    data class Line(val from: Coords, val to: Coords) {
        override fun toString(): String {
            return "$from->$to"
        }
    }

    fun part1() : Int {
        println("Day 5 part 1")

        val lines = parseInput()
        val withoutDiagonal = lines.filter { it.from.x == it.to.x || it.from.y == it.to.y }

        val field = HashMap<Coords, Int>()
        fillField(withoutDiagonal, field)

        return field.filter { (_, count) -> count > 1 }.count()
    }

    fun part2() : Int {
        println("Day 5 part 2")

        val lines = parseInput()

        val field = HashMap<Coords, Int>()
        fillField(lines, field)

        return field.filter { (_, count) -> count > 1 }.count()
    }

    private fun print(field: HashMap<Coords, Int>) {
        val maxX = field.maxOf { (point, _) -> point.x }
        val maxY = field.maxOf { (point, _) -> point.y }

        for (x in (0..maxX)) {
            val line = (0..maxY).map {
                    y -> field.getOrDefault(Coords(y,x), 0) }.
                            joinToString("") { if (it == 0) "." else it.toString()
                    }
            println(line)
        }
    }

    private fun fillField(
        lines: List<Line>,
        field: HashMap<Coords, Int>
    ) {
        for (line in lines) {
            if (line.from.x == line.to.x) {
                for (y in makeSequence(line.from.y, line.to.y)) {
                    val coord = Coords(line.from.x, y)
                    field[coord] = field.getOrDefault(coord, 0) + 1
                }
            } else if (line.from.y == line.to.y) {
                for (x in makeSequence(line.from.x, line.to.x)) {
                    val coord = Coords(x, line.from.y)
                    field[coord] = field.getOrDefault(coord, 0) + 1
                }
            } else {
                for (point in linePoints(line)) {
                    field[point] = field.getOrDefault(point, 0) + 1
                }
            }
        }
    }

    private fun linePoints(line: Line) : List<Coords> {
        val xs = makeSequence(line.from.x, line.to.x)
        val ys = makeSequence(line.from.y, line.to.y)
        return xs.zip(ys).map { (x, y) -> Coords(x,y) }.toList()
    }

    private fun makeSequence(from: Int, to: Int): Sequence<Int> {
        if (from < to) {
            return (from..to).asSequence()
        }
        return (from downTo to).asSequence()
    }


    private fun parseInput(): List<Line> {
        return Day5Input.values.
            split("\n").
            map {
                line -> line.split(" -> ").map {
                    pair -> pair.split(",").
                        map { it.toInt() }.
                        let { Coords(it[0], it[1]) }
                }.let {
                    Line(it[0], it[1])
                }
            }
    }
}
