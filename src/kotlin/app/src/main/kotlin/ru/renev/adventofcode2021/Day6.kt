package ru.renev.adventofcode2021

class Day6 {
    data class LanternsGroup(val timer: Int, val count: Long) {
        fun nextGroup() : List<LanternsGroup> {
            if (timer != 0) {
                return listOf(LanternsGroup(timer - 1, count))
            }

            return listOf(
                LanternsGroup(6, count),
                LanternsGroup(8, count)
            )
        }
    }

    fun part1() : Long {
        println("Day 6 part 1")

        return calculateCount(80)
    }

    fun part2() : Long {
        println("Day 6 part 2")

        return calculateCount(256)
    }

    private fun calculateCount(days: Int) : Long {
        var lanterns = parseInput().groupBy { it }.
            map { LanternsGroup(it.key, it.value.count().toLong()) }.
            toList()

        repeat(days) { _ ->
            lanterns = lanterns.flatMap { it.nextGroup() }.
            groupBy { it.timer }.
            map { grp -> LanternsGroup(grp.key, grp.value.sumOf { it.count }) }.
            toList()
        }

        return lanterns.fold(0L) { a, b -> a + b.count }
    }

    private fun parseInput(): List<Int> {
        return Day6Input.input.split(",").map { it.toInt() }
    }
}
